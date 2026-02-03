using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.UpdatePaiementFournisseur;

public class UpdatePaiementFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/paiement-fournisseur/{id:int}", HandleUpdatePaiementFournisseurAsync)
            .RequireAuthorization($"Permission:{Permissions.UpdatePaymentFournisseur}")
            .WithTags(EndpointTags.PaiementFournisseur);
    }

    public async Task<Results<NoContent, ValidationProblem>> HandleUpdatePaiementFournisseurAsync(
        IMediator mediator,
        int id,
        UpdatePaiementFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePaiementFournisseurCommand(
            id,
            request.NumeroTransactionBancaire,
            request.FournisseurId,
            request.AccountingYearId,
            request.Montant,
            request.DatePaiement,
            request.MethodePaiement,
            request.FactureFournisseurIds,
            request.BonDeReceptionIds,
            request.NumeroChequeTraite,
            request.BanqueId,
            request.DateEcheance,
            request.Commentaire,
            request.RibCodeEtab,
            request.RibCodeAgence,
            request.RibNumeroCompte,
            request.RibCle,
            request.Mois,
            request.DocumentBase64);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

