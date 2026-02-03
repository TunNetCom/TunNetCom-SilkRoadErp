using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Exceptions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.GetPaiementsFournisseur;

public class GetPaiementsFournisseurQueryHandler(
    SalesContext _context,
    IMemoryCache _cache,
    ILogger<GetPaiementsFournisseurQueryHandler> _logger)
    : IRequestHandler<GetPaiementsFournisseurQuery, Result<PagedList<PaiementFournisseurResponse>>>
{
    public async Task<Result<PagedList<PaiementFournisseurResponse>>> Handle(GetPaiementsFournisseurQuery query, CancellationToken cancellationToken)
    {
        var accountingYearIdsList = query.AccountingYearIds?.ToList();
        _logger.LogInformation("Fetching PaiementsFournisseur with filters FournisseurId={FournisseurId}, AccountingYearIds={AccountingYearIds}, DateEcheanceFrom={DateEcheanceFrom}, DateEcheanceTo={DateEcheanceTo}, MontantMin={MontantMin}, MontantMax={MontantMax}, HasNumeroTransactionBancaire={HasNumeroTransactionBancaire}, Mois={Mois}",
            query.FournisseurId, accountingYearIdsList != null ? string.Join(",", accountingYearIdsList) : null, query.DateEcheanceFrom, query.DateEcheanceTo, query.MontantMin, query.MontantMax, query.HasNumeroTransactionBancaire, query.Mois);

        var paiementsQuery = _context.PaiementFournisseur
            .AsNoTracking()
            .AsQueryable();

        if (query.FournisseurId.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.FournisseurId == query.FournisseurId.Value);
        }

        if (accountingYearIdsList != null && accountingYearIdsList.Count > 0)
        {
            paiementsQuery = paiementsQuery.Where(p => accountingYearIdsList.Contains(p.AccountingYearId));
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

        if (query.HasNumeroTransactionBancaire.HasValue)
        {
            if (query.HasNumeroTransactionBancaire.Value)
            {
                paiementsQuery = paiementsQuery.Where(p => !string.IsNullOrEmpty(p.NumeroTransactionBancaire));
            }
            else
            {
                paiementsQuery = paiementsQuery.Where(p => string.IsNullOrEmpty(p.NumeroTransactionBancaire));
            }
        }

        if (query.Mois.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.Mois.HasValue && p.Mois.Value == query.Mois.Value);
        }

        if (query.PageNumber < 1 || query.PageSize < 1)
        {
            throw new InvalidPaginationParamsException();
        }

        var activeAccountingYearId = SalesContext.GetActiveAccountingYearId();
        var countCacheKey = BuildCountCacheKey(query, activeAccountingYearId);

        var totalCount = await _cache.GetOrCreateAsync(countCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            return await paiementsQuery.CountAsync(cancellationToken);
        });

        var paiements = paiementsQuery
            .Select(p => new PaiementFournisseurResponse
            {
                Id = p.Id,
                NumeroTransactionBancaire = p.NumeroTransactionBancaire,
                FournisseurId = p.FournisseurId,
                FournisseurNom = p.Fournisseur.Nom,
                AccountingYearId = p.AccountingYearId,
                AccountingYear = p.AccountingYear.Year,
                Montant = p.Montant,
                DatePaiement = p.DatePaiement,
                MethodePaiement = p.MethodePaiement.ToString(),
                FactureFournisseurIds = p.FactureFournisseurs.Select(f => f.FactureFournisseurId).ToList(),
                BonDeReceptionIds = p.BonDeReceptions.Select(b => b.BonDeReceptionId).ToList(),
                NumeroChequeTraite = p.NumeroChequeTraite,
                BanqueId = p.BanqueId,
                BanqueNom = p.Banque != null ? p.Banque.Nom : null,
                DateEcheance = p.DateEcheance,
                Commentaire = p.Commentaire,
                RibCodeEtab = p.RibCodeEtab,
                RibCodeAgence = p.RibCodeAgence,
                RibNumeroCompte = p.RibNumeroCompte,
                RibCle = p.RibCle,
                // DocumentStoragePath excluded for performance - use GetPaiementFournisseurById for full details
                DocumentStoragePath = null,
                HasDocument = !string.IsNullOrEmpty(p.DocumentStoragePath),
                Mois = p.Mois,
                DateModification = p.DateModification
            })
            .OrderByDescending(p => p.DatePaiement);

        var items = await paiements
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedList<PaiementFournisseurResponse>(
            items,
            totalCount,
            query.PageNumber,
            query.PageSize);

        _logger.LogInformation("Fetched {Count} PaiementsFournisseur", pagedResult.TotalCount);
        return Result.Ok(pagedResult);
    }

    private static string BuildCountCacheKey(GetPaiementsFournisseurQuery query, int? activeAccountingYearId)
    {
        var accountingYearIdsKey = query.AccountingYearIds == null
            ? "null"
            : string.Join(",", query.AccountingYearIds.OrderBy(x => x));

        return string.Join("|",
            "PaiementFournisseurCount",
            $"activeYear:{activeAccountingYearId?.ToString() ?? "null"}",
            $"fournisseurId:{query.FournisseurId?.ToString() ?? "null"}",
            $"accountingYearIds:{accountingYearIdsKey}",
            $"dateEcheanceFrom:{query.DateEcheanceFrom?.ToString("O") ?? "null"}",
            $"dateEcheanceTo:{query.DateEcheanceTo?.ToString("O") ?? "null"}",
            $"montantMin:{query.MontantMin?.ToString() ?? "null"}",
            $"montantMax:{query.MontantMax?.ToString() ?? "null"}",
            $"hasNumeroTransactionBancaire:{query.HasNumeroTransactionBancaire?.ToString() ?? "null"}",
            $"mois:{query.Mois?.ToString() ?? "null"}");
    }
}

