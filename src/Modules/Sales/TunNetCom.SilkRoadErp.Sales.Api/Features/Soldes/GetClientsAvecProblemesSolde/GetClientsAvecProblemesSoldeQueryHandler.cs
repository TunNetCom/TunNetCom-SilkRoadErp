using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetClientsAvecProblemesSolde;

public class GetClientsAvecProblemesSoldeQueryHandler(
    SalesContext _context,
    ILogger<GetClientsAvecProblemesSoldeQueryHandler> _logger,
    IActiveAccountingYearService _activeAccountingYearService,
    ISoldeClientCalculationService _soldeClientCalculationService)
    : IRequestHandler<GetClientsAvecProblemesSoldeQuery, PagedList<ClientSoldeProblemeResponse>>
{
    public async Task<PagedList<ClientSoldeProblemeResponse>> Handle(
        GetClientsAvecProblemesSoldeQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetClientsAvecProblemesSoldeQuery called with PageNumber={PageNumber}, PageSize={PageSize}",
            query.PageNumber, query.PageSize);

        var accountingYearId = query.AccountingYearId;
        if (!accountingYearId.HasValue)
        {
            var activeYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
            if (!activeYearId.HasValue)
            {
                _logger.LogWarning("No active accounting year found");
                return new PagedList<ClientSoldeProblemeResponse>(new List<ClientSoldeProblemeResponse>(), 0, query.PageNumber, query.PageSize);
            }
            accountingYearId = activeYearId.Value;
        }

        var soldes = await _soldeClientCalculationService.GetSoldesClientsForAccountingYearAsync(accountingYearId.Value, cancellationToken);
        var clientIds = soldes.Select(x => x.ClientId).ToList();

        var quantitesParClient = new Dictionary<int, int>();
        if (clientIds.Any())
        {
            var quantitesNonLivrees = await _context.BonDeLivraison
                .Where(b => b.ClientId.HasValue && clientIds.Contains(b.ClientId.Value) && b.AccountingYearId == accountingYearId.Value)
                .Include(b => b.LigneBl)
                .ToListAsync(cancellationToken);

            quantitesParClient = quantitesNonLivrees
                .Where(b => b.ClientId.HasValue)
                .GroupBy(b => b.ClientId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.SelectMany(b => b.LigneBl)
                        .Select(l => l.QteLi - (l.QteLivree ?? l.QteLi))
                        .Where(q => q > 0)
                        .DefaultIfEmpty(0)
                        .Sum()
                );
        }

        var responses = soldes
            .Select(item =>
            {
                var nombreQuantitesNonLivrees = quantitesParClient.GetValueOrDefault(item.ClientId, 0);
                return new ClientSoldeProblemeResponse
                {
                    ClientId = item.ClientId,
                    ClientNom = item.ClientNom,
                    Solde = item.Solde,
                    NombreQuantitesNonLivrees = nombreQuantitesNonLivrees,
                    TotalFactures = item.TotalFactures,
                    TotalPaiements = item.TotalPaiements,
                    DateDernierDocument = item.DateDernierDocument
                };
            })
            .Where(x => x.Solde != 0 || x.NombreQuantitesNonLivrees > 0)
            .ToList();

        var totalCount = responses.Count;
        var pagedItems = responses
            .OrderByDescending(r => r.Solde < 0 ? 1 : 0)
            .ThenByDescending(r => r.NombreQuantitesNonLivrees)
            .ThenByDescending(r => r.DateDernierDocument ?? DateTime.MinValue)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        _logger.LogInformation("Found {Count} clients with solde problems", totalCount);

        return new PagedList<ClientSoldeProblemeResponse>(pagedItems, totalCount, query.PageNumber, query.PageSize);
    }
}
