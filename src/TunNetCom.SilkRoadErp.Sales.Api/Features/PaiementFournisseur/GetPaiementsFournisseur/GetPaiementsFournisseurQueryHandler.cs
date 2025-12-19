using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementsFournisseur;

public class GetPaiementsFournisseurQueryHandler(
    SalesContext _context,
    ILogger<GetPaiementsFournisseurQueryHandler> _logger)
    : IRequestHandler<GetPaiementsFournisseurQuery, Result<PagedList<PaiementFournisseurResponse>>>
{
    public async Task<Result<PagedList<PaiementFournisseurResponse>>> Handle(GetPaiementsFournisseurQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching PaiementsFournisseur with filters FournisseurId={FournisseurId}, AccountingYearId={AccountingYearId}, DateEcheanceFrom={DateEcheanceFrom}, DateEcheanceTo={DateEcheanceTo}, MontantMin={MontantMin}, MontantMax={MontantMax}", 
            query.FournisseurId, query.AccountingYearId, query.DateEcheanceFrom, query.DateEcheanceTo, query.MontantMin, query.MontantMax);

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

        if (query.DateEcheanceFrom.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.DateEcheance.HasValue && p.DateEcheance >= query.DateEcheanceFrom.Value);
        }

        if (query.DateEcheanceTo.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.DateEcheance.HasValue && p.DateEcheance <= query.DateEcheanceTo.Value);
        }

        if (query.MontantMin.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.Montant >= query.MontantMin.Value);
        }

        if (query.MontantMax.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.Montant <= query.MontantMax.Value);
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
                RibCodeEtab = p.RibCodeEtab,
                RibCodeAgence = p.RibCodeAgence,
                RibNumeroCompte = p.RibNumeroCompte,
                RibCle = p.RibCle,
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

