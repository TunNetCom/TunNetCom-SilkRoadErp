using Carter;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Dashboard.GetEvolutionVentesAchats;

public class GetEvolutionVentesAchatsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/dashboard/evolution", HandleAsync)
            .WithTags(EndpointTags.Dashboard)
            .Produces<EvolutionVentesAchatsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<Ok<EvolutionVentesAchatsResponse>, StatusCodeHttpResult>> HandleAsync(
        [FromServices] IMediator mediator,
        [FromServices] ILogger<GetEvolutionVentesAchatsEndpoint> logger,
        [FromQuery] int months = 12,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await mediator.Send(new GetEvolutionVentesAchatsQuery(months), cancellationToken);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting dashboard evolution");
            return TypedResults.StatusCode(500);
        }
    }
}
