using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementsFournisseur;

public class GetPaiementsFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetPaiementsFournisseurQueryHandler> _logger)
    : IRequestHandler<GetPaiementsFournisseurQuery, Result<PagedList<PaiementFournisseurResponse>>>
{
    public async Task<Result<PagedList<PaiementFournisseurResponse>>> Handle(GetPaiementsFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching PaiementsFournisseur with filters FournisseurId={FournisseurId}, AccountingYearId={AccountingYearId}", 
            query.FournisseurId, query.AccountingYearId);

        var paiementsQuery = _context.PaiementFournisseur
            .AsNoTracking()
            .AsQueryable();

        if (query.FournisseurId.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.FournisseurId == query.FournisseurId.Value);
        }

        if (query.AccountingYearId.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.AccountingYearId == query.AccountingYearId.Value);
        }

        var paiements = paiementsQuery
            .Select(p => new PaiementFournisseurResponse
            {
                Id = p.Id,
                Numero = p.Numero,
                FournisseurId = p.FournisseurId,
                FournisseurNom = p.Fournisseur.Nom,
                AccountingYearId = p.AccountingYearId,
                Montant = p.Montant,
                DatePaiement = p.DatePaiement,
                MethodePaiement = p.MethodePaiement.ToString(),
                FactureFournisseurId = p.FactureFournisseurId,
                BonDeReceptionId = p.BonDeReceptionId,
                NumeroChequeTraite = p.NumeroChequeTraite,
                BanqueId = p.BanqueId,
                BanqueNom = p.Banque != null ? p.Banque.Nom : null,
                DateEcheance = p.DateEcheance,
                Commentaire = p.Commentaire,
                DateModification = p.DateModification
            })
            .OrderByDescending(p => p.DatePaiement);

        var pagedResult = await PagedList<PaiementFournisseurResponse>.ToPagedListAsync(
            paiements,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        _logger.LogInformation("Fetched {Count} PaiementsFournisseur", pagedResult.TotalCount);
        return Result.Ok(pagedResult);
    }
}

