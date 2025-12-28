using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetClientsAvecProblemesSolde;

public class GetClientsAvecProblemesSoldeQueryHandler(
    SalesContext _context,
    ILogger<GetClientsAvecProblemesSoldeQueryHandler> _logger,
    IMediator mediator,
    IActiveAccountingYearService _activeAccountingYearService,
    IAccountingYearFinancialParametersService _financialParametersService)
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

        // Get timbre from financial parameters service
        var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

        // Get all clients with their related data in one query
        var clientsData = await _context.Client
            .AsNoTracking()
            .Select(client => new
            {
                Client = client,
                // Calculate total factures (without timbre)
                FacturesCount = _context.Facture
                    .Where(f => f.IdClient == client.Id && f.AccountingYearId == accountingYearId.Value)
                    .Count(),
                TotalFacturesNetPayer = _context.Facture
                    .Where(f => f.IdClient == client.Id && f.AccountingYearId == accountingYearId.Value)
                    .SelectMany(f => f.BonDeLivraison)
                    .Sum(b => (decimal?)b.NetPayer) ?? 0,
                // Calculate total non-factured BLs
                TotalBonsLivraisonNonFactures = _context.BonDeLivraison
                    .Where(b => b.ClientId == client.Id 
                        && b.AccountingYearId == accountingYearId.Value 
                        && b.NumFacture == null)
                    .Sum(b => (decimal?)b.NetPayer) ?? 0,
                // Calculate total avoirs
                TotalAvoirs = _context.Avoirs
                    .Where(a => a.ClientId == client.Id 
                        && a.AccountingYearId == accountingYearId.Value 
                        && a.NumFactureAvoirClient == null)
                    .SelectMany(a => a.LigneAvoirs)
                    .Sum(l => (decimal?)l.TotTtc) ?? 0,
                // Calculate total factures avoir
                TotalFacturesAvoir = _context.FactureAvoirClient
                    .Where(fa => fa.IdClient == client.Id && fa.AccountingYearId == accountingYearId.Value)
                    .SelectMany(fa => fa.Avoirs)
                    .SelectMany(a => a.LigneAvoirs)
                    .Sum(l => (decimal?)l.TotTtc) ?? 0,
                // Calculate total payments
                TotalPaiements = _context.PaiementClient
                    .Where(p => p.ClientId == client.Id && p.AccountingYearId == accountingYearId.Value)
                    .Sum(p => (decimal?)p.Montant) ?? 0,
                // Calculate quantités non livrées - we'll calculate this in memory after loading
                // because LINQ to SQL doesn't handle this complex calculation well
                NombreQuantitesNonLivrees = 0, // Will be calculated below
                // Get last document date
                DateDernierDocument = _context.Facture
                    .Where(f => f.IdClient == client.Id && f.AccountingYearId == accountingYearId.Value)
                    .Select(f => (DateTime?)f.Date)
                    .Concat(_context.BonDeLivraison
                        .Where(b => b.ClientId == client.Id 
                            && b.AccountingYearId == accountingYearId.Value 
                            && b.NumFacture == null)
                        .Select(b => (DateTime?)b.Date))
                    .Concat(_context.Avoirs
                        .Where(a => a.ClientId == client.Id 
                            && a.AccountingYearId == accountingYearId.Value 
                            && a.NumFactureAvoirClient == null)
                        .Select(a => (DateTime?)a.Date))
                    .Concat(_context.FactureAvoirClient
                        .Where(fa => fa.IdClient == client.Id && fa.AccountingYearId == accountingYearId.Value)
                        .Select(fa => (DateTime?)fa.Date))
                    .Max()
            })
            .ToListAsync(cancellationToken);

        // Calculate quantités non livrées for each client
        var clientIds = clientsData.Select(x => x.Client.Id).ToList();
        var quantitesParClient = new Dictionary<int, int>();
        
        if (clientIds.Any() && accountingYearId.HasValue)
        {
            var yearId = accountingYearId.Value;
            var quantitesNonLivrees = await _context.BonDeLivraison
                .Where(b => b.ClientId.HasValue && clientIds.Contains(b.ClientId.Value) && b.AccountingYearId == yearId)
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

        // Calculate solde and filter clients with problems in memory
        var responses = clientsData
            .Select(x =>
            {
                var totalFactures = x.TotalFacturesNetPayer + (timbre * x.FacturesCount);
                var solde = x.TotalAvoirs + x.TotalFacturesAvoir + x.TotalPaiements 
                    - totalFactures - x.TotalBonsLivraisonNonFactures;
                
                var nombreQuantitesNonLivrees = quantitesParClient.GetValueOrDefault(x.Client.Id, 0);

                return new ClientSoldeProblemeResponse
                {
                    ClientId = x.Client.Id,
                    ClientNom = x.Client.Nom,
                    Solde = solde,
                    NombreQuantitesNonLivrees = nombreQuantitesNonLivrees,
                    TotalFactures = totalFactures,
                    TotalPaiements = x.TotalPaiements,
                    DateDernierDocument = x.DateDernierDocument
                };
            })
            .Where(x => x.Solde < 0 || x.NombreQuantitesNonLivrees > 0)
            .ToList();

        // Apply pagination manually since we need to filter after calculation
        var totalCount = responses.Count;
        var pagedItems = responses
            .OrderByDescending(r => r.Solde < 0 ? 1 : 0) // Clients with negative balance first
            .ThenByDescending(r => r.NombreQuantitesNonLivrees) // Then by quantity issues
            .ThenByDescending(r => r.DateDernierDocument ?? DateTime.MinValue) // Then by last document date
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        _logger.LogInformation("Found {Count} clients with solde problems", totalCount);

        return new PagedList<ClientSoldeProblemeResponse>(pagedItems, totalCount, query.PageNumber, query.PageSize);
    }
}

