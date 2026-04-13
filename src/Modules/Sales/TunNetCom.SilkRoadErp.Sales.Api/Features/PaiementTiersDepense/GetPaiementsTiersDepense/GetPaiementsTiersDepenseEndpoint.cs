using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.GetPaiementsTiersDepense;

public class GetPaiementsTiersDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/paiements-tiers-depenses", async (
            IMediator mediator,
            int? tiersDepenseFonctionnementId = null,
            int? accountingYearId = null,
            DateTime? datePaiementFrom = null,
            DateTime? datePaiementTo = null,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default) =>
        {
            var query = new GetPaiementsTiersDepenseQuery(tiersDepenseFonctionnementId, accountingYearId, datePaiementFrom, datePaiementTo, pageNumber, pageSize);
            var result = await mediator.Send(query, cancellationToken);
            if (result.IsFailed)
                return Results.BadRequest(result.Errors);
            return Results.Ok(result.Value);
        })
        .RequireAuthorization($"Permission:{Permissions.ViewPaiementTiersDepense}")
        .WithTags(EndpointTags.PaiementTiersDepense);
    }
}
