using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.GetPaiementTiersDepense;

public class GetPaiementTiersDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/paiements-tiers-depenses/{id:int}", async (IMediator mediator, int id, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetPaiementTiersDepenseQuery(id), cancellationToken);
            if (result.IsEntityNotFound())
                return Results.NotFound();
            return Results.Ok(result.Value);
        })
        .RequireAuthorization($"Permission:{Permissions.ViewPaiementTiersDepense}")
        .WithTags(EndpointTags.PaiementTiersDepense);
    }
}
