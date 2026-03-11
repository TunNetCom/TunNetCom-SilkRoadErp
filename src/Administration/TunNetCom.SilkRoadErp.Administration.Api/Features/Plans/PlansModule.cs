using Carter;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Administration.Contracts.Plans;
using TunNetCom.SilkRoadErp.Administration.Domain.Entities;

namespace TunNetCom.SilkRoadErp.Administration.Api.Features.Plans;

public class PlansModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/plans").WithTags("Plans");

        group.MapGet("/", async (AdminContext db) =>
        {
            var plans = await db.Plans
                .Where(p => p.IsActive)
                .Include(p => p.PlanBoundedContexts)
                    .ThenInclude(pbc => pbc.BoundedContext)
                .Include(p => p.PlanBoundedContexts)
                    .ThenInclude(pbc => pbc.PlanFeatures)
                    .ThenInclude(pf => pf.Feature)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            return Results.Ok(plans.Select(ToPlanDto).ToList());
        });

        group.MapGet("/{id:int}", async (int id, AdminContext db) =>
        {
            var plan = await db.Plans
                .Include(p => p.PlanBoundedContexts)
                    .ThenInclude(pbc => pbc.BoundedContext)
                .Include(p => p.PlanBoundedContexts)
                    .ThenInclude(pbc => pbc.PlanFeatures)
                    .ThenInclude(pf => pf.Feature)
                .FirstOrDefaultAsync(p => p.Id == id);

            return plan is null ? Results.NotFound() : Results.Ok(ToPlanDto(plan));
        });

        group.MapPost("/", async (CreatePlanDto request, AdminContext db) =>
        {
            var plan = new Plan
            {
                Name = request.Name,
                Description = request.Description,
                MaxUsers = request.MaxUsers,
                MaxStorageBytes = request.MaxStorageBytes,
                MonthlyPrice = request.MonthlyPrice,
                YearlyPrice = request.YearlyPrice,
                ApiRateLimitPerMinute = request.ApiRateLimitPerMinute,
                TrialDays = request.TrialDays,
                IsActive = true
            };
            db.Plans.Add(plan);
            await db.SaveChangesAsync();
            return Results.Created($"/plans/{plan.Id}", ToPlanDto(plan));
        });

        group.MapPut("/{id:int}", async (int id, UpdatePlanDto request, AdminContext db) =>
        {
            var plan = await db.Plans.FindAsync(id);
            if (plan is null) return Results.NotFound();

            plan.Name = request.Name;
            plan.Description = request.Description;
            plan.MaxUsers = request.MaxUsers;
            plan.MaxStorageBytes = request.MaxStorageBytes;
            plan.MonthlyPrice = request.MonthlyPrice;
            plan.YearlyPrice = request.YearlyPrice;
            plan.ApiRateLimitPerMinute = request.ApiRateLimitPerMinute;
            plan.TrialDays = request.TrialDays;
            await db.SaveChangesAsync();

            await db.Entry(plan).Collection(p => p.PlanBoundedContexts).LoadAsync();
            return Results.Ok(ToPlanDto(plan));
        });

        group.MapDelete("/{id:int}", async (int id, AdminContext db) =>
        {
            var plan = await db.Plans.FindAsync(id);
            if (plan is null) return Results.NotFound();

            plan.IsActive = false;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static PlanDto ToPlanDto(Plan p) => new(
        p.Id,
        p.Name,
        p.Description,
        p.MaxUsers,
        p.MaxStorageBytes,
        p.MonthlyPrice,
        p.YearlyPrice,
        p.ApiRateLimitPerMinute,
        p.TrialDays,
        p.PlanBoundedContexts.Select(pbc => new PlanBoundedContextDto(
            pbc.BoundedContext?.Key ?? "",
            pbc.BoundedContext?.Name ?? "",
            pbc.IncludesAllFeatures,
            pbc.PlanFeatures.Select(pf => pf.Feature?.Key ?? "").ToList()
        )).ToList());
}
