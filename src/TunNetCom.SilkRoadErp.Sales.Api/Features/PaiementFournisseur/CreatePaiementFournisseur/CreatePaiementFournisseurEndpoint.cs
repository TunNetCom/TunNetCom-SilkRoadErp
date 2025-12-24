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
        // #region agent log
        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "C", location = "CreatePaiementFournisseurEndpoint.cs:13", message = "Endpoint entry", data = new { requestNumero = request.Numero, requestFournisseurId = request.FournisseurId, requestFactureFournisseurIds = request.FactureFournisseurIds, requestBonDeReceptionIds = request.BonDeReceptionIds, factureFournisseurIdsCount = request.FactureFournisseurIds?.Count ?? 0, bonDeReceptionIdsCount = request.BonDeReceptionIds?.Count ?? 0 }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion

        var command = new CreatePaiementFournisseurCommand(
            request.Numero,
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

        // #region agent log
        System.IO.File.AppendAllText(@"d:\Workspaces\SilkRoad\TunNetCom-SilkRoadErp\.cursor\debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "run1", hypothesisId = "D", location = "CreatePaiementFournisseurEndpoint.cs:36", message = "After mediator.Send", data = new { resultIsFailed = result.IsFailed, resultValue = result.IsFailed ? (int?)null : result.Value, errors = result.IsFailed ? result.Errors.Select(e => new { message = e.Message, reason = e.Reasons?.Select(r => r.Message).ToList() }).ToList() : null }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion

        if (result.IsFailed)
        {
            return result.ToValidationProblem();
        }

        return TypedResults.Created($"/paiement-fournisseur/{result.Value}", request);
    }
}

