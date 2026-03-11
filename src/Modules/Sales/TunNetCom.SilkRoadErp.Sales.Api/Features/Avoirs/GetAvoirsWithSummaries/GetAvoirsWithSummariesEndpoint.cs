using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.GetAvoirsWithSummaries;

public class GetAvoirsWithSummariesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/avoirs/summaries",
            async Task<Results<Ok<GetAvoirsWithSummariesResponse>, BadRequest<ProblemDetails>>> (
                IMediator mediator,
                [AsParameters] GetAvoirsQueryParams queryParams,
                CancellationToken cancellationToken) =>
            {
                var query = new GetAvoirsWithSummariesQuery(
                    PageNumber: queryParams.PageNumber ?? 1,
                    PageSize: queryParams.PageSize ?? 10,
                    ClientId: queryParams.ClientId,
                    SortOrder: queryParams.SortOrder,
                    SortProperty: queryParams.SortProperty,
                    SearchKeyword: queryParams.SearchKeyword,
                    StartDate: queryParams.StartDate,
                    EndDate: queryParams.EndDate,
                    Status: queryParams.Status
                );

                var response = await mediator.Send(query, cancellationToken);

                return TypedResults.Ok(response);
            })
            .WithName("GetAvoirsWithSummaries")
            .WithTags(EndpointTags.Avoirs)
            .Produces<GetAvoirsWithSummariesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
            .WithDescription("Gets a paginated list of avoirs with summaries, optionally filtered by client ID, date range, and search keyword.");
    }
}

