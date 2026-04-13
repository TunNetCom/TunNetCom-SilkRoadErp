using Carter;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Administration.Api.Infrastructure.Provisioning;
using TunNetCom.SilkRoadErp.Administration.Domain.Entities;
using TunNetCom.SilkRoadErp.Administration.Domain.Enums;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Administration.Api.Features.Registration;

public class RegistrationModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/register").WithTags("Registration");

        group.MapPost("/", async (
            RegisterTenantRequest request,
            AdminContext db,
            TenantProvisioningService provisioning) =>
        {
            var existing = await db.Tenants.AnyAsync(t => t.Identifier == request.Identifier);
            if (existing) return Results.Conflict(new { error = "Tenant identifier already exists" });

            var tenant = new Tenant
            {
                Identifier = request.Identifier,
                Name = request.CompanyName,
                Strategy = TenancyStrategy.SharedDatabaseSharedSchema,
                ConnectionString = "DefaultConnection",
                Status = TenantStatus.Provisioning
            };
            db.Tenants.Add(tenant);

            if (request.PlanId.HasValue)
            {
                var subscription = new Subscription
                {
                    TenantId = tenant.Id,
                    PlanId = request.PlanId.Value,
                    Status = SubscriptionStatus.Trial,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(14)
                };
                db.Subscriptions.Add(subscription);
            }

            var customerAccount = new CustomerAccount
            {
                TenantId = tenant.Id,
                Email = request.AdminEmail,
                Name = request.AdminName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.AdminPassword),
                IsOwner = true
            };
            db.CustomerAccounts.Add(customerAccount);

            if (request.BoundedContextIds is { Count: > 0 } bcIds)
            {
                var validIds = await db.BoundedContexts
                    .Where(bc => bcIds.Contains(bc.Id))
                    .Select(bc => bc.Id)
                    .ToListAsync();
                foreach (var bcId in validIds)
                {
                    db.TenantBoundedContexts.Add(new TenantBoundedContext
                    {
                        TenantId = tenant.Id,
                        BoundedContextId = bcId,
                        IsEnabled = true
                    });
                }
            }

            await db.SaveChangesAsync();

            return Results.Accepted($"/tenants/{tenant.Id}", new { tenantId = tenant.Id });
        });
    }
}

public record RegisterTenantRequest(
    string Identifier,
    string CompanyName,
    string AdminName,
    string AdminEmail,
    string AdminPassword,
    int? PlanId,
    string? Currency,
    string? Language,
    IReadOnlyList<int>? BoundedContextIds);
