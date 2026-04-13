using Carter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Administration.Api.Infrastructure.Provisioning;
using TunNetCom.SilkRoadErp.Administration.Contracts.Tenants;
using TunNetCom.SilkRoadErp.Administration.Domain.Entities;
using TunNetCom.SilkRoadErp.Administration.Domain.Enums;

namespace TunNetCom.SilkRoadErp.Administration.Api.Features.Tenants;

public class TenantsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tenants").WithTags("Tenants");

        group.MapGet("/", async (AdminContext db) =>
        {
            var tenants = await db.Tenants
                .Select(t => new TenantSummaryDto(t.Id, t.Identifier, t.Name, (int)t.Status, t.CreatedAt))
                .ToListAsync();
            return Results.Ok(tenants);
        });

        group.MapGet("/{id}", async (string id, AdminContext db) =>
        {
            var tenant = await db.Tenants
                .Include(t => t.Subscriptions).ThenInclude(s => s.Plan)
                .Include(t => t.TenantBoundedContexts).ThenInclude(tbc => tbc.BoundedContext)
                .FirstOrDefaultAsync(t => t.Id == id);

            return tenant is null ? Results.NotFound() : Results.Ok(tenant);
        });

        group.MapPost("/", async (CreateTenantRequest request, AdminContext db) =>
        {
            var tenant = new Tenant
            {
                Identifier = request.Identifier,
                Name = request.Name,
                Strategy = request.Strategy,
                ConnectionString = request.ConnectionString ?? "DefaultConnection",
                SchemaName = request.SchemaName,
                Status = TenantStatus.Provisioning
            };
            db.Tenants.Add(tenant);
            await db.SaveChangesAsync();
            return Results.Created($"/tenants/{tenant.Id}", tenant);
        });

        group.MapPost("/{id}/block", async (string id, BlockTenantRequest request, AdminContext db) =>
        {
            var tenant = await db.Tenants.FindAsync(id);
            if (tenant is null) return Results.NotFound();

            tenant.Status = TenantStatus.Blocked;
            tenant.BlockReason = request.Reason;
            tenant.BlockedAt = DateTime.UtcNow;
            tenant.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        group.MapPost("/{id}/unblock", async (string id, AdminContext db) =>
        {
            var tenant = await db.Tenants.FindAsync(id);
            if (tenant is null) return Results.NotFound();

            tenant.Status = TenantStatus.Active;
            tenant.BlockReason = null;
            tenant.BlockedAt = null;
            tenant.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok();
        });

        group.MapPut("/{id}", async (string id, UpdateTenantDto request, AdminContext db) =>
        {
            var tenant = await db.Tenants.FindAsync(id);
            if (tenant is null) return Results.NotFound();

            tenant.Name = request.Name;
            if (request.ConnectionString is not null)
                tenant.ConnectionString = request.ConnectionString;
            tenant.SchemaName = request.SchemaName;
            tenant.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.Ok(new TenantSummaryDto(tenant.Id, tenant.Identifier, tenant.Name, (int)tenant.Status, tenant.CreatedAt));
        });

        group.MapDelete("/{id}", async (string id, AdminContext db) =>
        {
            var tenant = await db.Tenants.FindAsync(id);
            if (tenant is null) return Results.NotFound();

            tenant.Status = TenantStatus.Deactivated;
            tenant.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapGet("/resolve/{identifier}", async (string identifier, AdminContext db) =>
        {
            var tenant = await db.Tenants
                .Include(t => t.TenantBoundedContexts)
                .Include(t => t.TenantFeatureOverrides)
                .FirstOrDefaultAsync(t => t.Identifier == identifier);

            if (tenant is null) return Results.NotFound();

            return Results.Ok(new
            {
                tenant.Id,
                tenant.Identifier,
                tenant.Name,
                tenant.Strategy,
                tenant.ConnectionString,
                tenant.SchemaName,
                IsActive = tenant.Status == TenantStatus.Active,
                tenant.Status,
                EnabledBoundedContexts = tenant.TenantBoundedContexts
                    .Where(tbc => tbc.IsEnabled)
                    .Select(tbc => tbc.BoundedContextId)
            });
        });

        group.MapPost("/{id}/provision", async (string id, StartProvisioningRequest request, AdminContext db, IServiceScopeFactory scopeFactory) =>
        {
            var tenant = await db.Tenants.FindAsync(id);
            if (tenant is null) return Results.NotFound();

            if (tenant.Status != TenantStatus.Provisioning)
                return Results.BadRequest(new { error = "Tenant is not in Provisioning state." });

            if (string.IsNullOrWhiteSpace(request.ConnectionId))
                return Results.BadRequest(new { error = "ConnectionId is required." });

            var connectionId = request.ConnectionId;
            _ = Task.Run(async () =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var provisioning = scope.ServiceProvider.GetRequiredService<TenantProvisioningService>();
                try
                {
                    await provisioning.ProvisionAsync(id, connectionId);
                }
                catch
                {
                    // Logged inside ProvisioningService; client gets failure via SignalR
                }
            });

            return Results.Accepted($"/tenants/{id}/provision", new { message = "Provisioning started" });
        });
    }
}

public record CreateTenantRequest(
    string Identifier,
    string Name,
    TunNetCom.SilkRoadErp.SharedKernel.Tenancy.TenancyStrategy Strategy,
    string? ConnectionString,
    string? SchemaName);

public record BlockTenantRequest(string Reason);

public record StartProvisioningRequest(string ConnectionId);
