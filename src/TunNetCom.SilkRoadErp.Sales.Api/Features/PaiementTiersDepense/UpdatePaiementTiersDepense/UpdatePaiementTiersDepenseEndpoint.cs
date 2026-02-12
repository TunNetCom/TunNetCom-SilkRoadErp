using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.UpdatePaiementTiersDepense;

public class UpdatePaiementTiersDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/paiements-tiers-depenses/{id:int}", async (IMediator mediator, int id, UpdatePaiementTiersDepenseRequest request, CancellationToken cancellationToken) =>
        {
            var command = new UpdatePaiementTiersDepenseCommand(
                id,
                request.NumeroTransactionBancaire,
                request.TiersDepenseFonctionnementId,
                request.AccountingYearId,
                request.Montant,
                request.DatePaiement,
                request.MethodePaiement ?? string.Empty,
                request.FactureDepenseIds,
                request.NumeroChequeTraite,
                request.BanqueId,
                request.DateEcheance,
                request.Commentaire,
                request.RibCodeEtab,
                request.RibCodeAgence,
                request.RibNumeroCompte,
                request.RibCle,
                request.DocumentBase64,
                request.Mois);
            var result = await mediator.Send(command, cancellationToken);
            if (result.IsFailed)
                return result.IsEntityNotFound() ? Results.NotFound() : Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .RequireAuthorization($"Permission:{Permissions.UpdatePaiementTiersDepense}")
        .WithTags(EndpointTags.PaiementTiersDepense);
    }
}
