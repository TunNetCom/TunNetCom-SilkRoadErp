using Carter;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Dashboard.GetRecapVentesAchats;

public class GetRecapVentesAchatsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/dashboard/recap-ventes-achats", HandleAsync)
            .WithTags(EndpointTags.Dashboard)
            .Produces<RecapVentesAchatsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<Ok<RecapVentesAchatsResponse>, StatusCodeHttpResult>> HandleAsync(
        [FromServices] IMediator mediator,
        [FromServices] ILogger<GetRecapVentesAchatsEndpoint> logger,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await mediator.Send(new GetRecapVentesAchatsQuery(startDate, endDate), cancellationToken);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting recap ventes/achats");
            return TypedResults.StatusCode(500);
        }
    }
}
