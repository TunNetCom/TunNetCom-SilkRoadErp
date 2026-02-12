using Carter;
using Microsoft.AspNetCore.Mvc;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoices.GetProviderInvoiceTotals;

public class GetProviderInvoiceTotalsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/provider-invoices/totals", HandleGetTotalsAsync)
            .WithTags(EndpointTags.ProviderInvoices)
            .Produces<ProviderInvoiceTotalsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<Ok<ProviderInvoiceTotalsResponse>, StatusCodeHttpResult>> HandleGetTotalsAsync(
        [FromServices] IMediator mediator,
        [FromServices] ILogger<GetProviderInvoiceTotalsEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? providerId = null,
        [FromQuery] int[]? tagIds = null,
        [FromQuery] int? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "GetProviderInvoiceTotalsEndpoint called with startDate: {StartDate}, endDate: {EndDate}, providerId: {ProviderId}, tagIds: {TagIds}, status: {Status}",
                startDate, endDate, providerId, tagIds != null ? string.Join(",", tagIds) : "null", status);

            var response = await mediator.Send(new GetProviderInvoiceTotalsQuery(startDate, endDate, providerId, tagIds, status), cancellationToken);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating provider invoice totals");
            return TypedResults.StatusCode(500);
        }
    }
}

