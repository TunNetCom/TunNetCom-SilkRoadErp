using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Soldes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class SoldeClientCalculationService(
    SalesContext _context,
    IMediator _mediator,
    IAccountingYearFinancialParametersService _financialParametersService,
    ILogger<SoldeClientCalculationService> _logger) : ISoldeClientCalculationService
{
    public async Task<SoldeClientCalculDto?> CalculateSoldeClientAsync(
        int clientId,
        int accountingYearId,
        CancellationToken cancellationToken = default)
    {
        var exists = await _context.Client
            .AsNoTracking()
            .AnyAsync(c => c.Id == clientId, cancellationToken);
        if (!exists)
            return null;

        var yearExists = await _context.AccountingYear
            .AsNoTracking()
            .AnyAsync(ay => ay.Id == accountingYearId, cancellationToken);
        if (!yearExists)
            return null;

        var appParams = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

        var retenues = await _context.RetenueSourceClient
            .Where(r => _context.Facture
                .Where(f => f.IdClient == clientId && f.AccountingYearId == accountingYearId)
                .Select(f => f.Num)
                .Contains(r.NumFacture))
            .ToDictionaryAsync(r => r.NumFacture, cancellationToken);

        var factures = await _context.Facture
            .Where(f => f.IdClient == clientId && f.AccountingYearId == accountingYearId)
            .Include(f => f.BonDeLivraison)
            .ToListAsync(cancellationToken);

        var montantApresRetenuByNum = retenues.ToDictionary(r => r.Key, r => r.Value.MontantApresRetenu);
        var totalFactures = factures.Sum(f =>
            SoldeClientCalculator.ComputeMontantFactureClient(
                f.Num, montantApresRetenuByNum,
                f.BonDeLivraison.Sum(b => b.NetPayer), timbre));

        var totalBonsLivraisonNonFactures = await _context.BonDeLivraison
            .Where(b => b.ClientId == clientId
                && b.AccountingYearId == accountingYearId
                && b.NumFacture == null)
            .SumAsync(b => b.NetPayer, cancellationToken);

        var totalAvoirs = await _context.Avoirs
            .Where(a => a.ClientId == clientId
                && a.AccountingYearId == accountingYearId
                && a.NumFactureAvoirClient == null)
            .SelectMany(a => a.LigneAvoirs)
            .SumAsync(l => l.TotTtc, cancellationToken);

        var totalFacturesAvoir = await _context.FactureAvoirClient
            .Where(fa => fa.IdClient == clientId && fa.AccountingYearId == accountingYearId)
            .SelectMany(fa => fa.Avoirs)
            .SelectMany(a => a.LigneAvoirs)
            .SumAsync(l => l.TotTtc, cancellationToken);

        var totalPaiements = await _context.PaiementClient
            .Where(p => p.ClientId == clientId && p.AccountingYearId == accountingYearId)
            .SumAsync(p => p.Montant, cancellationToken);

        var solde = SoldeClientCalculator.ComputeSolde(
            totalFactures, totalBonsLivraisonNonFactures, totalAvoirs, totalFacturesAvoir, totalPaiements);

        return new SoldeClientCalculDto
        {
            TotalFactures = totalFactures,
            TotalBonsLivraisonNonFactures = totalBonsLivraisonNonFactures,
            TotalAvoirs = totalAvoirs,
            TotalFacturesAvoir = totalFacturesAvoir,
            TotalPaiements = totalPaiements,
            Solde = solde
        };
    }

    public async Task<IReadOnlyList<SoldeClientItemDto>> GetSoldesClientsForAccountingYearAsync(
        int accountingYearId,
        CancellationToken cancellationToken = default)
    {
        var appParams = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

        var clientIdsWithActivity = await GetClientIdsWithActivityAsync(accountingYearId, cancellationToken);
        if (clientIdsWithActivity.Count == 0)
        {
            _logger.LogDebug("No clients with activity in accounting year {AccountingYearId}", accountingYearId);
            return Array.Empty<SoldeClientItemDto>();
        }

        var totalFacturesByClient = await GetTotalFacturesByClientAsync(accountingYearId, timbre, cancellationToken);

        var aggregates = await _context.Client
            .AsNoTracking()
            .Where(c => clientIdsWithActivity.Contains(c.Id))
            .Select(c => new
            {
                c.Id,
                c.Nom,
                TotalBonsLivraisonNonFactures = _context.BonDeLivraison
                    .Where(b => b.ClientId == c.Id
                        && b.AccountingYearId == accountingYearId
                        && b.NumFacture == null)
                    .Sum(b => (decimal?)b.NetPayer) ?? 0,
                TotalAvoirs = _context.Avoirs
                    .Where(a => a.ClientId == c.Id
                        && a.AccountingYearId == accountingYearId
                        && a.NumFactureAvoirClient == null)
                    .SelectMany(a => a.LigneAvoirs)
                    .Sum(l => (decimal?)l.TotTtc) ?? 0,
                TotalFacturesAvoir = _context.FactureAvoirClient
                    .Where(fa => fa.IdClient == c.Id && fa.AccountingYearId == accountingYearId)
                    .SelectMany(fa => fa.Avoirs)
                    .SelectMany(a => a.LigneAvoirs)
                    .Sum(l => (decimal?)l.TotTtc) ?? 0,
                TotalPaiements = _context.PaiementClient
                    .Where(p => p.ClientId == c.Id && p.AccountingYearId == accountingYearId)
                    .Sum(p => (decimal?)p.Montant) ?? 0,
                DateDernierDocument = _context.Facture
                    .Where(f => f.IdClient == c.Id && f.AccountingYearId == accountingYearId)
                    .Select(f => (DateTime?)f.Date)
                    .Concat(_context.BonDeLivraison
                        .Where(b => b.ClientId == c.Id
                            && b.AccountingYearId == accountingYearId
                            && b.NumFacture == null)
                        .Select(b => (DateTime?)b.Date))
                    .Concat(_context.Avoirs
                        .Where(a => a.ClientId == c.Id
                            && a.AccountingYearId == accountingYearId
                            && a.NumFactureAvoirClient == null)
                        .Select(a => (DateTime?)a.Date))
                    .Concat(_context.FactureAvoirClient
                        .Where(fa => fa.IdClient == c.Id && fa.AccountingYearId == accountingYearId)
                        .Select(fa => (DateTime?)fa.Date))
                    .Concat(_context.PaiementClient
                        .Where(p => p.ClientId == c.Id && p.AccountingYearId == accountingYearId)
                        .Select(p => (DateTime?)p.DatePaiement))
                    .Max()
            })
            .ToListAsync(cancellationToken);

        var result = new List<SoldeClientItemDto>(aggregates.Count);
        foreach (var a in aggregates)
        {
            var totalFactures = totalFacturesByClient.GetValueOrDefault(a.Id, 0m);
            var solde = SoldeClientCalculator.ComputeSolde(
                totalFactures, a.TotalBonsLivraisonNonFactures, a.TotalAvoirs, a.TotalFacturesAvoir, a.TotalPaiements);
            result.Add(new SoldeClientItemDto
            {
                ClientId = a.Id,
                ClientNom = a.Nom ?? string.Empty,
                TotalFactures = totalFactures,
                TotalBonsLivraisonNonFactures = a.TotalBonsLivraisonNonFactures,
                TotalAvoirs = a.TotalAvoirs,
                TotalFacturesAvoir = a.TotalFacturesAvoir,
                TotalPaiements = a.TotalPaiements,
                Solde = solde,
                DateDernierDocument = a.DateDernierDocument
            });
        }

        return result;
    }

    private async Task<HashSet<int>> GetClientIdsWithActivityAsync(int accountingYearId, CancellationToken cancellationToken)
    {
        var fromFactures = await _context.Facture
            .Where(f => f.AccountingYearId == accountingYearId)
            .Select(f => f.IdClient)
            .Distinct()
            .ToListAsync(cancellationToken);
        var fromBL = await _context.BonDeLivraison
            .Where(b => b.AccountingYearId == accountingYearId && b.NumFacture == null)
            .Where(b => b.ClientId != null)
            .Select(b => b.ClientId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
        var fromAvoirs = await _context.Avoirs
            .Where(a => a.AccountingYearId == accountingYearId)
            .Where(a => a.ClientId != null)
            .Select(a => a.ClientId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
        var fromFacturesAvoir = await _context.FactureAvoirClient
            .Where(fa => fa.AccountingYearId == accountingYearId)
            .Select(fa => fa.IdClient)
            .Distinct()
            .ToListAsync(cancellationToken);
        var fromPaiements = await _context.PaiementClient
            .Where(p => p.AccountingYearId == accountingYearId)
            .Select(p => p.ClientId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var set = new HashSet<int>();
        foreach (var id in fromFactures.Where(id => id != 0)) set.Add(id);
        foreach (var id in fromBL) set.Add(id);
        foreach (var id in fromAvoirs) set.Add(id);
        foreach (var id in fromFacturesAvoir) set.Add(id);
        foreach (var id in fromPaiements) set.Add(id);
        return set;
    }

    private async Task<Dictionary<int, decimal>> GetTotalFacturesByClientAsync(
        int accountingYearId,
        decimal timbre,
        CancellationToken cancellationToken)
    {
        var retenues = await _context.RetenueSourceClient
            .Where(r => r.AccountingYearId == accountingYearId)
            .ToDictionaryAsync(r => r.NumFacture, cancellationToken);

        var factures = await _context.Facture
            .Where(f => f.AccountingYearId == accountingYearId)
            .Include(f => f.BonDeLivraison)
            .ToListAsync(cancellationToken);

        var montantApresRetenuByNum = retenues.ToDictionary(r => r.Key, r => r.Value.MontantApresRetenu);
        return factures
            .GroupBy(f => f.IdClient)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(f => SoldeClientCalculator.ComputeMontantFactureClient(
                    f.Num, montantApresRetenuByNum,
                    f.BonDeLivraison.Sum(b => b.NetPayer), timbre)));
    }
}
