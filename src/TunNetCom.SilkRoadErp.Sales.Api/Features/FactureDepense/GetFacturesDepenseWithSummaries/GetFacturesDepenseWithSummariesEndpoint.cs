using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureDepense.GetFacturesDepenseWithSummaries;

public class GetFacturesDepenseWithSummariesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/factures-depenses", async (
            IMediator mediator,
            int pageNumber = 1,
            int pageSize = 10,
            int? tiersDepenseFonctionnementId = null,
            int? accountingYearId = null,
            string? searchKeyword = null,
            CancellationToken cancellationToken = default) =>
        {
            var query = new GetFacturesDepenseWithSummariesQuery(
                pageNumber,
                pageSize,
                tiersDepenseFonctionnementId,
                accountingYearId,
                searchKeyword);
            var result = await mediator.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .RequireAuthorization($"Permission:{Permissions.ViewFactureDepense}")
        .WithTags(EndpointTags.FactureDepense);
    }
}
