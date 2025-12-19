using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.GetPaiementsClient;

public class GetPaiementsClientQueryHandler(
    SalesContext _context,
    ILogger<GetPaiementsClientQueryHandler> _logger)
    : IRequestHandler<GetPaiementsClientQuery, Result<PagedList<PaiementClientResponse>>>
{
    public async Task<Result<PagedList<PaiementClientResponse>>> Handle(GetPaiementsClientQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching PaiementsClient with filters ClientId={ClientId}, AccountingYearId={AccountingYearId}, DateEcheanceFrom={DateEcheanceFrom}, DateEcheanceTo={DateEcheanceTo}, MontantMin={MontantMin}, MontantMax={MontantMax}", 
            query.ClientId, query.AccountingYearId, query.DateEcheanceFrom, query.DateEcheanceTo, query.MontantMin, query.MontantMax);

        var paiementsQuery = _context.PaiementClient
            .AsNoTracking()
            .AsQueryable();

        if (query.ClientId.HasValue)
        {
            paiementsQuery = paiementsQuery.Where(p => p.ClientId == query.ClientId.Value);
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
                DateModification = p.DateModification
            })
            .OrderByDescending(p => p.DatePaiement);

        var pagedResult = await PagedList<PaiementClientResponse>.ToPagedListAsync(
            paiements,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        _logger.LogInformation("Fetched {Count} PaiementsClient", pagedResult.TotalCount);
        return Result.Ok(pagedResult);
    }
}

