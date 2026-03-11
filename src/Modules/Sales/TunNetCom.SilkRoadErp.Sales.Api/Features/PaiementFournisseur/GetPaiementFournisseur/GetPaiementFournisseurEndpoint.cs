using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementFournisseur;

public class GetPaiementFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/paiement-fournisseur/{id:int}", HandleGetPaiementFournisseurAsync)
            .RequireAuthorization($"Permission:{Permissions.ViewPaymentsFournisseur}")
            .RequireRateLimiting("paiement-fournisseur")
            .WithTags(EndpointTags.PaiementFournisseur);
    }

    public async Task<Results<Ok<PaiementFournisseurResponse>, NotFound>> HandleGetPaiementFournisseurAsync(
        IMediator mediator,
        int id,
        CancellationToken cancellationToken)
    {
        var query = new GetPaiementFournisseurQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.Value);
    }
}

