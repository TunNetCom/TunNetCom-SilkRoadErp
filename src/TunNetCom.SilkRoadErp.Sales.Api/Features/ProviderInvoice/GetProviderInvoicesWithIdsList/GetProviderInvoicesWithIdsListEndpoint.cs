using TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithIds;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetProviderInvoicesWithIdsList;

public class GetProviderInvoicesWithIdsListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/provider-invoices/byids",
                async Task<List<ProviderInvoiceResponse>> (
                    IMediator mediator,
                    [FromBody] List<int> invoicesIds,
                    CancellationToken cancellationToken) =>
                {
                    var query = new GetProviderInvoicesWithIdsListQuery(invoicesIds);
                    var result = await mediator.Send(query, cancellationToken);

                    return result;
                })
            .WithName("GetProviderInvoicesByIds")
            .Produces<List<ProviderInvoiceResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }
}

