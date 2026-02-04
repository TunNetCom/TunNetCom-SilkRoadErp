using Carter;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;
using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementTiersDepense.CreatePaiementTiersDepense;

public class CreatePaiementTiersDepenseEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/paiements-tiers-depenses", async (IMediator mediator, CreatePaiementTiersDepenseRequest request, CancellationToken cancellationToken) =>
        {
            var command = new CreatePaiementTiersDepenseCommand(
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
            return Results.Created($"/paiements-tiers-depenses/{result.Value}", new { id = result.Value });
        })
        .RequireAuthorization($"Permission:{Permissions.CreatePaiementTiersDepense}")
        .WithTags(EndpointTags.PaiementTiersDepense);
    }
}
