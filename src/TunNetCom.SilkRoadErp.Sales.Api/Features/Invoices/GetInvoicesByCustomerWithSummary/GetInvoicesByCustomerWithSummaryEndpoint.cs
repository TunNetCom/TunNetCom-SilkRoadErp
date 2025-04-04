namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomerWithSummary;

public class GetInvoicesByCustomerWithSummaryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/invoices/client/{clientId}",
                async Task<IResult> (
                    IMediator mediator,
                    int clientId,
                    [FromQuery] int pageNumber,
                    [FromQuery] int pageSize,
                    [FromQuery] string sortOrder,
                    [FromQuery] string sortProprety,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetInvoicesByCustomerWithSummaryQuery(clientId, pageNumber, pageSize, sortProprety, sortOrder);

                    var result = await mediator.Send(query, cancellationToken);

                    return Results.Ok(result.Value);
                });
    }
}
