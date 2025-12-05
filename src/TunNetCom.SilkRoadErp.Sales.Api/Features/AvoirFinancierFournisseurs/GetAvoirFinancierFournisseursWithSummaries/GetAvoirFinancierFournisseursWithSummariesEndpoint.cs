using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFinancierFournisseurs.GetAvoirFinancierFournisseursWithSummaries;

public class GetAvoirFinancierFournisseursWithSummariesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/avoir-financier-fournisseurs", HandleGetAvoirFinancierFournisseursWithSummariesAsync)
            .WithTags(EndpointTags.AvoirFinancierFournisseurs);
    }

    public async Task<Ok<GetAvoirFinancierFournisseursWithSummariesResponse>> HandleGetAvoirFinancierFournisseursWithSummariesAsync(
        IMediator mediator,
        [AsParameters] GetAvoirFinancierFournisseursQueryParams queryParams,
        CancellationToken cancellationToken)
    {
        var query = new GetAvoirFinancierFournisseursWithSummariesQuery(
            ProviderId: queryParams.ProviderId,
            NumFactureFournisseur: queryParams.NumFactureFournisseur,
            SortOrder: queryParams.SortOrder,
            SortProperty: queryParams.SortProperty,
            PageNumber: queryParams.PageNumber ?? 1,
            PageSize: queryParams.PageSize ?? 10,
            SearchKeyword: queryParams.SearchKeyword,
            StartDate: queryParams.StartDate,
            EndDate: queryParams.EndDate);

        var response = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(response);
    }
}

