using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFactureAvoirFournisseurWithSummaries;

public class GetFactureAvoirFournisseurWithSummariesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/facture-avoir-fournisseur/summaries",
            async Task<Results<Ok<GetFactureAvoirFournisseurWithSummariesResponse>, BadRequest<ProblemDetails>>> (
                IMediator mediator,
                [AsParameters] GetFactureAvoirFournisseurQueryParams queryParams,
                CancellationToken cancellationToken) =>
            {
                var query = new GetFactureAvoirFournisseurWithSummariesQuery(
                    PageNumber: queryParams.PageNumber ?? 1,
                    PageSize: queryParams.PageSize ?? 10,
                    IdFournisseur: queryParams.IdFournisseur,
                    NumFactureFournisseur: queryParams.NumFactureFournisseur,
                    SortOrder: queryParams.SortOrder,
                    SortProperty: queryParams.SortProperty,
                    SearchKeyword: queryParams.SearchKeyword,
                    StartDate: queryParams.StartDate,
                    EndDate: queryParams.EndDate
                );

                var response = await mediator.Send(query, cancellationToken);

                return TypedResults.Ok(response);
            })
            .WithName("GetFactureAvoirFournisseurWithSummaries")
            .WithTags(EndpointTags.FactureAvoirFournisseur)
            .Produces<GetFactureAvoirFournisseurWithSummariesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
            .WithDescription("Gets a paginated list of facture avoir fournisseurs with summaries, optionally filtered by fournisseur ID, facture fournisseur, date range, and search keyword.");
    }
}

