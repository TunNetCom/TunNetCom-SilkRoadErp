using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Soldes;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class SoldeFournisseurCalculationService(
    SalesContext _context,
    IMediator _mediator,
    IAccountingYearFinancialParametersService _financialParametersService,
    ILogger<SoldeFournisseurCalculationService> _logger) : ISoldeFournisseurCalculationService
{
    public async Task<SoldeFournisseurCalculDto?> CalculateSoldeFournisseurAsync(
        int fournisseurId,
        int accountingYearId,
        CancellationToken cancellationToken = default)
    {
        var exists = await _context.Fournisseur
            .AsNoTracking()
            .AnyAsync(f => f.Id == fournisseurId, cancellationToken);
        if (!exists)
            return null;

        var yearExists = await _context.AccountingYear
            .AsNoTracking()
            .AnyAsync(ay => ay.Id == accountingYearId, cancellationToken);
        if (!yearExists)
            return null;

        var appParams = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

        var facturesFournisseur = await _context.FactureFournisseur
            .Where(f => f.IdFournisseur == fournisseurId && f.AccountingYearId == accountingYearId)
            .Include(f => f.BonDeReception)
                .ThenInclude(br => br.LigneBonReception)
            .ToListAsync(cancellationToken);

        var totalFactures = facturesFournisseur.Sum(f =>
            f.BonDeReception.SelectMany(br => br.LigneBonReception).Sum(l => l.TotTtc) + timbre);

        var totalBonsReceptionNonFactures = await _context.BonDeReception
            .Where(b => b.IdFournisseur == fournisseurId
                && b.AccountingYearId == accountingYearId
                && b.NumFactureFournisseur == null)
            .SelectMany(br => br.LigneBonReception)
            .SumAsync(l => l.TotTtc, cancellationToken);

        var totalFacturesAvoir = await _context.FactureAvoirFournisseur
            .Where(fa => fa.IdFournisseur == fournisseurId && fa.AccountingYearId == accountingYearId)
            .SelectMany(fa => fa.AvoirFournisseur)
            .SelectMany(a => a.LigneAvoirFournisseur)
            .SumAsync(l => l.TotTtc, cancellationToken);

        var totalAvoirsFinanciers = await (from af in _context.AvoirFinancierFournisseurs
                                           join ff in _context.FactureFournisseur on af.NumFactureFournisseur equals ff.Num
                                           where ff.IdFournisseur == fournisseurId && ff.AccountingYearId == accountingYearId
                                           select af.TotTtc).SumAsync(cancellationToken);

        var totalPaiements = await _context.PaiementFournisseur
            .Where(p => p.FournisseurId == fournisseurId && p.AccountingYearId == accountingYearId)
            .SumAsync(p => p.Montant, cancellationToken);

        var solde = SoldeFournisseurCalculator.ComputeSolde(totalFactures, totalBonsReceptionNonFactures, totalFacturesAvoir, totalAvoirsFinanciers, totalPaiements);

        return new SoldeFournisseurCalculDto
        {
            TotalFactures = totalFactures,
            TotalBonsReceptionNonFactures = totalBonsReceptionNonFactures,
            TotalFacturesAvoir = totalFacturesAvoir,
            TotalAvoirsFinanciers = totalAvoirsFinanciers,
            TotalPaiements = totalPaiements,
            Solde = solde
        };
    }

    public async Task<IReadOnlyList<SoldeFournisseurItemDto>> GetSoldesFournisseursForAccountingYearAsync(
        int accountingYearId,
        CancellationToken cancellationToken = default)
    {
        var appParams = await _mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

        var fournisseurIdsWithActivity = await GetFournisseurIdsWithActivityAsync(accountingYearId, cancellationToken);
        if (fournisseurIdsWithActivity.Count == 0)
        {
            _logger.LogDebug("No fournisseurs with activity in accounting year {AccountingYearId}", accountingYearId);
            return Array.Empty<SoldeFournisseurItemDto>();
        }

        var totalFacturesByFournisseur = await GetTotalFacturesByFournisseurAsync(accountingYearId, timbre, cancellationToken);

        var aggregates = await _context.Fournisseur
            .AsNoTracking()
            .Where(f => fournisseurIdsWithActivity.Contains(f.Id))
            .Select(f => new
            {
                f.Id,
                f.Nom,
                TotalBonsReceptionNonFactures = _context.BonDeReception
                    .Where(b => b.IdFournisseur == f.Id && b.AccountingYearId == accountingYearId && b.NumFactureFournisseur == null)
                    .SelectMany(b => b.LigneBonReception)
                    .Sum(l => (decimal?)l.TotTtc) ?? 0,
                TotalFacturesAvoir = _context.FactureAvoirFournisseur
                    .Where(fa => fa.IdFournisseur == f.Id && fa.AccountingYearId == accountingYearId)
                    .SelectMany(fa => fa.AvoirFournisseur)
                    .SelectMany(a => a.LigneAvoirFournisseur)
                    .Sum(l => (decimal?)l.TotTtc) ?? 0,
                TotalAvoirsFinanciers = (from af in _context.AvoirFinancierFournisseurs
                                         join ff in _context.FactureFournisseur on af.NumFactureFournisseur equals ff.Num
                                         where ff.IdFournisseur == f.Id && ff.AccountingYearId == accountingYearId
                                         select (decimal?)af.TotTtc).Sum() ?? 0,
                TotalPaiements = _context.PaiementFournisseur
                    .Where(p => p.FournisseurId == f.Id && p.AccountingYearId == accountingYearId)
                    .Sum(p => (decimal?)p.Montant) ?? 0,
                DateDernierDocument = _context.FactureFournisseur
                    .Where(ff => ff.IdFournisseur == f.Id && ff.AccountingYearId == accountingYearId)
                    .Select(ff => (DateTime?)ff.Date)
                    .Concat(_context.BonDeReception
                        .Where(b => b.IdFournisseur == f.Id && b.AccountingYearId == accountingYearId && b.NumFactureFournisseur == null)
                        .Select(b => (DateTime?)b.Date))
                    .Concat(_context.FactureAvoirFournisseur
                        .Where(fa => fa.IdFournisseur == f.Id && fa.AccountingYearId == accountingYearId)
                        .Select(fa => (DateTime?)fa.Date))
                    .Concat(_context.PaiementFournisseur
                        .Where(p => p.FournisseurId == f.Id && p.AccountingYearId == accountingYearId)
                        .Select(p => (DateTime?)p.DatePaiement))
                    .Max()
            })
            .ToListAsync(cancellationToken);

        var result = new List<SoldeFournisseurItemDto>(aggregates.Count);
        foreach (var a in aggregates)
        {
            var totalFactures = totalFacturesByFournisseur.GetValueOrDefault(a.Id, 0m);
            var solde = SoldeFournisseurCalculator.ComputeSolde(totalFactures, a.TotalBonsReceptionNonFactures, a.TotalFacturesAvoir, a.TotalAvoirsFinanciers, a.TotalPaiements);
            result.Add(new SoldeFournisseurItemDto
            {
                FournisseurId = a.Id,
                FournisseurNom = a.Nom ?? string.Empty,
                TotalFactures = totalFactures,
                TotalBonsReceptionNonFactures = a.TotalBonsReceptionNonFactures,
                TotalFacturesAvoir = a.TotalFacturesAvoir,
                TotalAvoirsFinanciers = a.TotalAvoirsFinanciers,
                TotalPaiements = a.TotalPaiements,
                Solde = solde,
                DateDernierDocument = a.DateDernierDocument
            });
        }

        return result;
    }

    private async Task<HashSet<int>> GetFournisseurIdsWithActivityAsync(int accountingYearId, CancellationToken cancellationToken)
    {
        var fromFactures = await _context.FactureFournisseur
            .Where(f => f.AccountingYearId == accountingYearId)
            .Select(f => f.IdFournisseur)
            .Distinct()
            .ToListAsync(cancellationToken);
        var fromBR = await _context.BonDeReception
            .Where(b => b.AccountingYearId == accountingYearId)
            .Select(b => b.IdFournisseur)
            .Distinct()
            .ToListAsync(cancellationToken);
        var fromFacturesAvoir = await _context.FactureAvoirFournisseur
            .Where(fa => fa.AccountingYearId == accountingYearId)
            .Select(fa => fa.IdFournisseur)
            .Distinct()
            .ToListAsync(cancellationToken);
        var fromPaiements = await _context.PaiementFournisseur
            .Where(p => p.AccountingYearId == accountingYearId)
            .Select(p => p.FournisseurId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var fromAvoirsFinanciers = await (from af in _context.AvoirFinancierFournisseurs
                                          join ff in _context.FactureFournisseur on af.NumFactureFournisseur equals ff.Num
                                          where ff.AccountingYearId == accountingYearId
                                          select ff.IdFournisseur)
            .Distinct()
            .ToListAsync(cancellationToken);

        var set = new HashSet<int>(fromFactures);
        foreach (var id in fromBR) set.Add(id);
        foreach (var id in fromFacturesAvoir) set.Add(id);
        foreach (var id in fromPaiements) set.Add(id);
        foreach (var id in fromAvoirsFinanciers) set.Add(id);
        return set;
    }

    private async Task<Dictionary<int, decimal>> GetTotalFacturesByFournisseurAsync(
        int accountingYearId,
        decimal timbre,
        CancellationToken cancellationToken)
    {
        var factures = await _context.FactureFournisseur
            .Where(f => f.AccountingYearId == accountingYearId)
            .Include(f => f.BonDeReception)
                .ThenInclude(br => br.LigneBonReception)
            .ToListAsync(cancellationToken);

        return factures
            .GroupBy(f => f.IdFournisseur)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(f => f.BonDeReception.SelectMany(br => br.LigneBonReception).Sum(l => l.TotTtc) + timbre));
    }
}
