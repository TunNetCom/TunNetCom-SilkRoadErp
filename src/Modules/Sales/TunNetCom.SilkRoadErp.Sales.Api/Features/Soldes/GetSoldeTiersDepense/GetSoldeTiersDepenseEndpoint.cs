using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeTiersDepense;

public class GetSoldeTiersDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/soldes/tiers-depenses/{tiersDepenseFonctionnementId:int}", async (
            IMediator mediator,
            int tiersDepenseFonctionnementId,
            int? accountingYearId,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSoldeTiersDepenseQuery(tiersDepenseFonctionnementId, accountingYearId);
            var result = await mediator.Send(query, cancellationToken);
            if (result.IsFailed)
                return result.IsEntityNotFound() ? Results.NotFound() : Results.BadRequest(result.Errors);
            return Results.Ok(result.Value);
        })
        .RequireAuthorization($"Permission:{Permissions.ViewSoldeTiersDepense}")
        .WithTags(EndpointTags.Soldes);
    }
}
