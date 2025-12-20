using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetPaiementClient;

public class GetPaiementClientQueryHandler(
    SalesContext _context,
    ILogger<GetPaiementClientQueryHandler> _logger)
    : IRequestHandler<GetPaiementClientQuery, Result<PaiementClientResponse>>
{
    public async Task<Result<PaiementClientResponse>> Handle(GetPaiementClientQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching PaiementClient with Id {Id}", query.Id);

        var paiement = await _context.PaiementClient
            .AsNoTracking()
            .Where(p => p.Id == query.Id)
            .Select(p => new PaiementClientResponse
            {
                Id = p.Id,
                Numero = p.Numero,
                ClientId = p.ClientId,
                ClientNom = p.Client.Nom,
                AccountingYearId = p.AccountingYearId,
                Montant = p.Montant,
                DatePaiement = p.DatePaiement,
                MethodePaiement = p.MethodePaiement.ToString(),
                FactureId = p.FactureId,
                BonDeLivraisonId = p.BonDeLivraisonId,
                NumeroChequeTraite = p.NumeroChequeTraite,
                BanqueId = p.BanqueId,
                BanqueNom = p.Banque != null ? p.Banque.Nom : null,
                DateEcheance = p.DateEcheance,
                Commentaire = p.Commentaire,
                DocumentStoragePath = p.DocumentStoragePath,
                DateModification = p.DateModification
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (paiement == null)
        {
            _logger.LogWarning("PaiementClient with Id {Id} not found", query.Id);
            return Result.Fail("paiement_client_not_found");
        }

        _logger.LogInformation("PaiementClient with Id {Id} fetched successfully", query.Id);
        return Result.Ok(paiement);
    }
}

