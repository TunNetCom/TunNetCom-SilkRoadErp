namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.GetProvidersInvoices;

public class GetProvidersInvoicesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/provider-invoice",
            async Task<Ok<GetProviderInvoicesWithSummary>> (
                IMediator mediator,
                [AsParameters] GetProvidersInvoicesQuery queryParams,
                CancellationToken cancellationToken) =>
            {
                var query = new GetProvidersInvoicesQuery(
                    IdFournisseur: queryParams.IdFournisseur,
                    PageNumber: queryParams.PageNumber,
                    PageSize: queryParams.PageSize,
                    SearchKeyword: queryParams.SearchKeyword,
                    SortOrder: queryParams.SortOrder,
                    SortProperty: queryParams.SortProperty
                );
                var response = await mediator.Send(query, cancellationToken);
                return TypedResults.Ok(response);
            })
            .WithName("GetProvidersInvoices")
            .Produces<GetProviderInvoicesWithSummary>(StatusCodes.Status200OK)
            .WithDescription("Gets a paginated list of provider invoices,  provider invoice status, and search keyword.");
        //TODO : Add problem details for 400 and 500 status codes
        
    }
}
