using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetAvoirFournisseurWithSummaries;

public class GetAvoirFournisseurWithSummariesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/avoir-fournisseur/summaries",
            async Task<Results<Ok<GetAvoirFournisseurWithSummariesResponse>, BadRequest<ProblemDetails>>> (
                IMediator mediator,
                [AsParameters] GetAvoirFournisseurQueryParams queryParams,
                CancellationToken cancellationToken) =>
            {
                var query = new GetAvoirFournisseurWithSummariesQuery(
                    PageNumber: queryParams.PageNumber ?? 1,
                    PageSize: queryParams.PageSize ?? 10,
                    FournisseurId: queryParams.FournisseurId,
                    NumFactureAvoirFournisseur: queryParams.NumFactureAvoirFournisseur,
                    SortOrder: queryParams.SortOrder,
                    SortProperty: queryParams.SortProperty,
                    SearchKeyword: queryParams.SearchKeyword,
                    StartDate: queryParams.StartDate,
                    EndDate: queryParams.EndDate
                );

                var response = await mediator.Send(query, cancellationToken);

                return TypedResults.Ok(response);
            })
            .WithName("GetAvoirFournisseurWithSummaries")
            .WithTags(EndpointTags.AvoirFournisseur)
            .Produces<GetAvoirFournisseurWithSummariesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
            .WithDescription("Gets a paginated list of avoir fournisseurs with summaries, optionally filtered by fournisseur ID, facture avoir fournisseur, date range, and search keyword.");
    }
}

