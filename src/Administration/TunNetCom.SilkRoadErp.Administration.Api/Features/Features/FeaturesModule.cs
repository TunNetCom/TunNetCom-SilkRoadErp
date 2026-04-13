using Carter;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Administration.Contracts.BoundedContexts;
using TunNetCom.SilkRoadErp.Administration.Domain.Entities;

namespace TunNetCom.SilkRoadErp.Administration.Api.Features.Features;

public class FeaturesModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/bounded-contexts/{bcId:int}/features").WithTags("Features");

        group.MapGet("/", async (int bcId, AdminContext db) =>
        {
            var features = await db.Features
                .Where(f => f.BoundedContextId == bcId)
                .OrderBy(f => f.Key)
                .Select(f => new FeatureSummaryDto(f.Id, f.Key, f.Name, f.Description, f.IsCore))
                .ToListAsync();
            return Results.Ok(features);
        });

        group.MapPost("/", async (int bcId, CreateFeatureDto request, AdminContext db) =>
        {
            var bcExists = await db.BoundedContexts.AnyAsync(b => b.Id == bcId);
            if (!bcExists) return Results.NotFound("Bounded context not found.");

            var feature = new Feature
            {
                BoundedContextId = bcId,
                Key = request.Key,
                Name = request.Name,
                Description = request.Description,
                IsCore = request.IsCore
            };
            db.Features.Add(feature);
            await db.SaveChangesAsync();
            return Results.Created($"/bounded-contexts/{bcId}/features/{feature.Id}",
                new FeatureSummaryDto(feature.Id, feature.Key, feature.Name, feature.Description, feature.IsCore));
        });

        group.MapPut("/{id:int}", async (int bcId, int id, UpdateFeatureDto request, AdminContext db) =>
        {
            var feature = await db.Features.FirstOrDefaultAsync(f => f.Id == id && f.BoundedContextId == bcId);
            if (feature is null) return Results.NotFound();

            feature.Name = request.Name;
            feature.Description = request.Description;
            feature.IsCore = request.IsCore;
            await db.SaveChangesAsync();
            return Results.Ok(new FeatureSummaryDto(feature.Id, feature.Key, feature.Name, feature.Description, feature.IsCore));
        });

        group.MapDelete("/{id:int}", async (int bcId, int id, AdminContext db) =>
        {
            var feature = await db.Features
                .Include(f => f.PlanFeatures)
                .Include(f => f.TenantFeatureOverrides)
                .FirstOrDefaultAsync(f => f.Id == id && f.BoundedContextId == bcId);

            if (feature is null) return Results.NotFound();

            if (feature.PlanFeatures.Count > 0 || feature.TenantFeatureOverrides.Count > 0)
                return Results.Conflict("Cannot delete feature that is referenced by plans or tenants.");

            db.Features.Remove(feature);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
