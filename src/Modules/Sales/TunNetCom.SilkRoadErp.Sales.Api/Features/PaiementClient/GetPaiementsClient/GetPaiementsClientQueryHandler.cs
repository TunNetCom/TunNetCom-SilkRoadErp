using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetPaiementsClient;

public class GetPaiementsClientQueryHandler(
    SalesContext _context,
    ILogger<GetPaiementsClientQueryHandler> _logger)
    : IRequestHandler<GetPaiementsClientQuery, Result<PagedList<PaiementClientResponse>>>
{
    public async Task<Result<PagedList<PaiementClientResponse>>> Handle(GetPaiementsClientQuery query, CancellationToken cancellationToken)
    {
        var accountingYearIdsList = query.AccountingYearIds?.ToList();
        _logger.LogInformation("Fetching PaiementsClient with filters ClientId={ClientId}, AccountingYearIds={AccountingYearIds}, DatePaiementFrom={DatePaiementFrom}, DatePaiementTo={DatePaiementTo}, DateEcheanceFrom={DateEcheanceFrom}, DateEcheanceTo={DateEcheanceTo}, MontantMin={MontantMin}, MontantMax={MontantMax}, HasNumeroTransactionBancaire={HasNumeroTransactionBancaire}",
            query.ClientId, accountingYearIdsList != null ? string.Join(",", accountingYearIdsList) : null, query.DatePaiementFrom, query.DatePaiementTo, query.DateEcheanceFrom, query.DateEcheanceTo, query.MontantMin, query.MontantMax, query.HasNumeroTransactionBancaire);

        var paiementsQuery = _context.PaiementClient
            .AsNoTracking()
            .AsQueryable();

        if (query.ClientId.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.ClientId == query.ClientId.Value);
        }

        if (accountingYearIdsList != null && accountingYearIdsList.Count > 0)
        {
            paiementsQuery = paiementsQuery.Where(p => accountingYearIdsList.Contains(p.AccountingYearId));
        }

        if (query.DatePaiementFrom.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.DatePaiement >= query.DatePaiementFrom.Value);
        }

        if (query.DatePaiementTo.HasValue)
        {
            var endDateInclusive = query.DatePaiementTo.Value.Date.AddDays(1).AddTicks(-1);
            paiementsQuery = paiementsQuery.Where(p => p.DatePaiement <= endDateInclusive);
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

        if (query.BonDeLivraisonId.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.BonDeLivraisons.Any(b => b.BonDeLivraisonId == query.BonDeLivraisonId.Value));
        }

        if (query.FactureId.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.Factures.Any(f => f.FactureId == query.FactureId.Value));
        }

        var paiements = paiementsQuery
            .Select(p => new PaiementClientResponse
            {
                Id = p.Id,
                NumeroTransactionBancaire = p.NumeroTransactionBancaire,
                ClientId = p.ClientId,
                ClientNom = p.Client.Nom,
                AccountingYearId = p.AccountingYearId,
                AccountingYear = p.AccountingYear.Year,
                Montant = p.Montant,
                DatePaiement = p.DatePaiement,
                MethodePaiement = p.MethodePaiement.ToString(),
                FactureIds = p.Factures.Select(f => f.FactureId).ToList(),
                BonDeLivraisonIds = p.BonDeLivraisons.Select(b => b.BonDeLivraisonId).ToList(),
                NumeroChequeTraite = p.NumeroChequeTraite,
                BanqueId = p.BanqueId,
                BanqueNom = p.Banque != null ? p.Banque.Nom : null,
                DateEcheance = p.DateEcheance,
                Commentaire = p.Commentaire,
                DocumentStoragePath = p.DocumentStoragePath,
                DateModification = p.DateModification
            })
            .OrderByDescending(p => p.DatePaiement);

        try
        {
            var pagedResult = await PagedList<PaiementClientResponse>.ToPagedListAsync(
                paiements,
                query.PageNumber,
                query.PageSize,
                cancellationToken);

            _logger.LogInformation("Fetched {Count} PaiementsClient", pagedResult.TotalCount);
            return Result.Ok(pagedResult);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

