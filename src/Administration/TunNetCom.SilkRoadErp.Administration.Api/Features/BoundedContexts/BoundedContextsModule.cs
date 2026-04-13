using Carter;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Administration.Contracts.BoundedContexts;
using TunNetCom.SilkRoadErp.Administration.Domain.Entities;

namespace TunNetCom.SilkRoadErp.Administration.Api.Features.BoundedContexts;

public class BoundedContextsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/bounded-contexts").WithTags("BoundedContexts");

        group.MapGet("/", async (AdminContext db) =>
        {
            var list = await db.BoundedContexts
                .OrderBy(bc => bc.DisplayOrder)
                .ThenBy(bc => bc.Key)
                .Select(bc => new BoundedContextSummaryDto(
                    bc.Id,
                    bc.Key,
                    bc.Name,
                    bc.Description,
                    bc.Icon,
                    bc.IsCore,
                    bc.DisplayOrder,
                    bc.Features.Count))
                .ToListAsync();
            return Results.Ok(list);
        });

        group.MapGet("/{id:int}", async (int id, AdminContext db) =>
        {
            var bc = await db.BoundedContexts
                .Include(b => b.Features)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bc is null) return Results.NotFound();

            return Results.Ok(new BoundedContextDetailDto(
                bc.Id, bc.Key, bc.Name, bc.Description, bc.Icon, bc.IsCore, bc.DisplayOrder,
                bc.Features.Select(f => new FeatureSummaryDto(f.Id, f.Key, f.Name, f.Description, f.IsCore)).ToList()));
        });

        group.MapPost("/", async (CreateBoundedContextDto request, AdminContext db) =>
        {
            var bc = new BoundedContext
            {
                Key = request.Key,
                Name = request.Name,
                Description = request.Description,
                Icon = request.Icon,
                IsCore = request.IsCore,
                DisplayOrder = request.DisplayOrder
            };
            db.BoundedContexts.Add(bc);
            await db.SaveChangesAsync();
            return Results.Created($"/bounded-contexts/{bc.Id}",
                new BoundedContextSummaryDto(bc.Id, bc.Key, bc.Name, bc.Description, bc.Icon, bc.IsCore, bc.DisplayOrder, 0));
        });

        group.MapPut("/{id:int}", async (int id, UpdateBoundedContextDto request, AdminContext db) =>
        {
            var bc = await db.BoundedContexts.FindAsync(id);
            if (bc is null) return Results.NotFound();

            bc.Name = request.Name;
            bc.Description = request.Description;
            bc.Icon = request.Icon;
            bc.IsCore = request.IsCore;
            bc.DisplayOrder = request.DisplayOrder;
            await db.SaveChangesAsync();
            return Results.Ok(new BoundedContextSummaryDto(bc.Id, bc.Key, bc.Name, bc.Description, bc.Icon, bc.IsCore, bc.DisplayOrder, 0));
        });

        group.MapDelete("/{id:int}", async (int id, AdminContext db) =>
        {
            var bc = await db.BoundedContexts
                .Include(b => b.PlanBoundedContexts)
                .Include(b => b.TenantBoundedContexts)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bc is null) return Results.NotFound();

            if (bc.PlanBoundedContexts.Count > 0 || bc.TenantBoundedContexts.Count > 0)
                return Results.Conflict("Cannot delete bounded context that is referenced by plans or tenants.");

            db.BoundedContexts.Remove(bc);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
