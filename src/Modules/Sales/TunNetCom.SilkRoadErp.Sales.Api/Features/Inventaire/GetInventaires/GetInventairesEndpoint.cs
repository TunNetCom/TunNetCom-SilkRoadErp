using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetInventaires;

public class GetInventairesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/inventaires",
            async (IMediator mediator,
            [AsParameters] GetInventairesQueryParams queryParams,
            CancellationToken cancellationToken) =>
            {
                var query = new GetInventairesQuery(
                    PageNumber: queryParams.PageNumber,
                    PageSize: queryParams.PageSize,
                    SearchKeyword: queryParams.SearchKeyword,
                    SortProperty: queryParams.SortProperty,
                    SortOrder: queryParams.SortOrder,
                    AccountingYearId: queryParams.AccountingYearId
                );

                var result = await mediator.Send(query, cancellationToken);
                return Results.Ok(result);
            })
            .WithTags("Inventaire");
    }
}

