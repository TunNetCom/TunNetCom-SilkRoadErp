using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.UpdatePaiementFournisseur;

public class UpdatePaiementFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/paiement-fournisseur/{id:int}", HandleUpdatePaiementFournisseurAsync)
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
            request.DocumentBase64);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}

