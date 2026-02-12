using Carter;
using Microsoft.AspNetCore.Mvc;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoiceTotals;

public class GetInvoiceTotalsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/invoices/totals", HandleGetTotalsAsync)
            .WithTags(EndpointTags.Invoices)
            .Produces<InvoiceTotalsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    public static async Task<Results<Ok<InvoiceTotalsResponse>, StatusCodeHttpResult>> HandleGetTotalsAsync(
        [FromServices] IMediator mediator,
        [FromServices] ILogger<GetInvoiceTotalsEndpoint> logger,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int[]? tagIds = null,
        [FromQuery] int? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(
                "GetInvoiceTotalsEndpoint called with startDate: {StartDate}, endDate: {EndDate}, customerId: {CustomerId}, tagIds: {TagIds}, status: {Status}",
                startDate, endDate, customerId, tagIds != null ? string.Join(",", tagIds) : "null", status);

            var response = await mediator.Send(new GetInvoiceTotalsQuery(startDate, endDate, customerId, tagIds, status), cancellationToken);
            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating invoice totals");
            return TypedResults.StatusCode(500);
        }
    }
}

