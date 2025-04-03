namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomer;

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
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetInvoicesByCustomerWithSummaryQuery(clientId, pageNumber, pageSize);

                    var result = await mediator.Send(query, cancellationToken);

                    return Results.Ok(result.Value);
                });
    }
}
