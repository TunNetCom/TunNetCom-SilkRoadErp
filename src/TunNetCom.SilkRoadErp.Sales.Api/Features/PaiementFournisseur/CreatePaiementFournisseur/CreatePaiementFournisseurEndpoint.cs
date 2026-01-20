using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.CreatePaiementFournisseur;

public class CreatePaiementFournisseurEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/paiement-fournisseur", HandleCreatePaiementFournisseurAsync)
            .WithTags(EndpointTags.PaiementFournisseur);
    }

    public async Task<Results<Created<CreatePaiementFournisseurRequest>, ValidationProblem>> HandleCreatePaiementFournisseurAsync(
        IMediator mediator,
        CreatePaiementFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaiementFournisseurCommand(
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
            request.Mois,
            request.DocumentBase64);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/paiement-fournisseur/{result.Value}", request);
    }
}

