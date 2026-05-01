using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesByCustomerWithSummary;

public class GetInvoicesByCustomerWithSummaryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
                "/invoices/client/{clientId}",
                async Task<IResult> (
                    IMediator mediator,
                    int clientId,
                    [FromQuery] int pageNumber,
                    [FromQuery] int pageSize,
                    [FromQuery] string sortOrder,
                    [FromQuery] string sortProprety,
                    [FromQuery] int? statut,
                    CancellationToken cancellationToken) =>
                {
                    DocumentStatus? documentStatus = statut.HasValue ? (DocumentStatus)statut.Value : null;
                    var query = new GetInvoicesByCustomerWithSummaryQuery(clientId, pageNumber, pageSize, sortProprety, sortOrder, documentStatus);

                    var result = await mediator.Send(query, cancellationToken);

                    return Results.Ok(result.Value);
                })
                .WithTags(EndpointTags.Invoices);
    }
}
