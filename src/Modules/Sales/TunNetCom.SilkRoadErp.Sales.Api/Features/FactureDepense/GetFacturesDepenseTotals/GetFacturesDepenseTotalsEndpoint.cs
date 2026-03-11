using Carter;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFacturesDepenseTotals;

public class GetFacturesDepenseTotalsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/factures-depenses/totals", HandleGetTotalsAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
            .WithTags(EndpointTags.FactureDepense)
            .Produces<FactureDepenseTotalsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<Ok<FactureDepenseTotalsResponse>, StatusCodeHttpResult>> HandleGetTotalsAsync(
        [FromServices] IMediator mediator,
        [FromServices] ILogger<GetFacturesDepenseTotalsEndpoint> logger,
        [FromQuery] int? accountingYearId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? tiersDepenseFonctionnementId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "GetFacturesDepenseTotalsEndpoint called with accountingYearId: {AccountingYearId}, startDate: {StartDate}, endDate: {EndDate}, tiersId: {TiersId}",
                accountingYearId, startDate, endDate, tiersDepenseFonctionnementId);

            var response = await mediator.Send(
                new GetFacturesDepenseTotalsQuery(accountingYearId, startDate, endDate, tiersDepenseFonctionnementId),
                cancellationToken);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating factures dépense totals");
            return TypedResults.StatusCode(500);
        }
    }
}
