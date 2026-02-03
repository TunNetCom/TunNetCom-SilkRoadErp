using TunNetCom.SilkRoadErp.Sales.Api.Features.AppParameters.GetAppParameters;
using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetRestesALivrerParClient;

public class GetRestesALivrerParClientQueryHandler(
    SalesContext _context,
    ILogger<GetRestesALivrerParClientQueryHandler> _logger,
    IMediator mediator,
    IActiveAccountingYearService _activeAccountingYearService,
    IAccountingYearFinancialParametersService _financialParametersService)
    : IRequestHandler<GetRestesALivrerParClientQuery, Result<RestesALivrerParClientResponse>>
{
    public async Task<Result<RestesALivrerParClientResponse>> Handle(
        GetRestesALivrerParClientQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetRestesALivrerParClientQuery called");

        var accountingYearId = query.AccountingYearId;
        if (!accountingYearId.HasValue)
        {
            var activeYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
            if (!activeYearId.HasValue)
            {
                _logger.LogWarning("No active accounting year found");
                return Result.Fail("no_active_accounting_year");
            }
            accountingYearId = activeYearId.Value;
        }

        var appParams = await mediator.Send(new GetAppParametersQuery(), cancellationToken);
        var timbre = await _financialParametersService.GetTimbreAsync(appParams.Value.Timbre, cancellationToken);

        var clientsData = await _context.Client
            .AsNoTracking()
            .Select(client => new
            {
                Client = client,
                FacturesCount = _context.Facture
                    .Where(f => f.IdClient == client.Id && f.AccountingYearId == accountingYearId.Value)
                    .Count(),
                TotalFacturesNetPayer = _context.Facture
                    .Where(f => f.IdClient == client.Id && f.AccountingYearId == accountingYearId.Value)
                    .SelectMany(f => f.BonDeLivraison)
                    .Sum(b => (decimal?)b.NetPayer) ?? 0,
                TotalBonsLivraisonNonFactures = _context.BonDeLivraison
                    .Where(b => b.ClientId == client.Id
                        && b.AccountingYearId == accountingYearId.Value
                        && b.NumFacture == null)
                    .Sum(b => (decimal?)b.NetPayer) ?? 0,
                TotalAvoirs = _context.Avoirs
                    .Where(a => a.ClientId == client.Id
                        && a.AccountingYearId == accountingYearId.Value
                        && a.NumFactureAvoirClient == null)
                    .SelectMany(a => a.LigneAvoirs)
                    .Sum(l => (decimal?)l.TotTtc) ?? 0,
                TotalFacturesAvoir = _context.FactureAvoirClient
                    .Where(fa => fa.IdClient == client.Id && fa.AccountingYearId == accountingYearId.Value)
                    .SelectMany(fa => fa.Avoirs)
                    .SelectMany(a => a.LigneAvoirs)
                    .Sum(l => (decimal?)l.TotTtc) ?? 0,
                TotalPaiements = _context.PaiementClient
                    .Where(p => p.ClientId == client.Id && p.AccountingYearId == accountingYearId.Value)
                    .Sum(p => (decimal?)p.Montant) ?? 0
            })
            .ToListAsync(cancellationToken);

        var clientIds = clientsData.Select(x => x.Client.Id).ToList();
        var quantitesParClient = new Dictionary<int, int>();

        if (clientIds.Any())
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

        var clientsWithProblems = clientsData
            .Select(x =>
            {
                var totalFactures = x.TotalFacturesNetPayer + (timbre * x.FacturesCount);
                var solde = x.TotalAvoirs + x.TotalFacturesAvoir + x.TotalPaiements
                    - totalFactures - x.TotalBonsLivraisonNonFactures;
                var nombreQuantitesNonLivrees = quantitesParClient.GetValueOrDefault(x.Client.Id, 0);
                return new { x.Client, Solde = solde, NombreQuantitesNonLivrees = nombreQuantitesNonLivrees };
            })
            .Where(x => x.Solde != 0 || x.NombreQuantitesNonLivrees > 0)
            .OrderByDescending(x => x.Solde < 0 ? 1 : 0)
            .ThenByDescending(x => x.NombreQuantitesNonLivrees)
            .ToList();

        if (!clientsWithProblems.Any())
        {
            return Result.Ok(new RestesALivrerParClientResponse { Clients = new List<ClientRestesALivrerItem>() });
        }

        var problemClientIds = clientsWithProblems.Select(x => x.Client.Id).ToList();
        var problemYearId = accountingYearId!.Value;
        var bonsLivraison = await _context.BonDeLivraison
            .Where(b => b.ClientId.HasValue && problemClientIds.Contains(b.ClientId.Value) && b.AccountingYearId == problemYearId)
            .Include(b => b.LigneBl)
            .ToListAsync(cancellationToken);

        var lignesGroupedByClientAndProduct = bonsLivraison
            .Where(b => b.ClientId.HasValue)
            .SelectMany(b => b.LigneBl.Select(l =>
            {
                var qteLivree = l.QteLivree ?? 0;
                var quantiteRestante = l.QteLi - qteLivree;
                return new { ClientId = b.ClientId!.Value, l.RefProduit, l.DesignationLi, QuantiteRestante = quantiteRestante };
            }))
            .Where(x => x.QuantiteRestante > 0)
            .GroupBy(x => new { x.ClientId, x.RefProduit })
            .Select(g => new
            {
                g.Key.ClientId,
                g.Key.RefProduit,
                DesignationLi = g.First().DesignationLi,
                QuantiteRestante = g.Sum(x => x.QuantiteRestante)
            })
            .ToList();

        var lignesParClient = lignesGroupedByClientAndProduct
            .GroupBy(x => x.ClientId)
            .ToDictionary(g => g.Key, g => g.Select(l => new LigneResteaLivrer
            {
                RefProduit = l.RefProduit,
                DesignationLi = l.DesignationLi,
                QuantiteRestante = l.QuantiteRestante
            }).ToList());

        var responseClients = clientsWithProblems.Select(c =>
        {
            var lignes = lignesParClient.GetValueOrDefault(c.Client.Id, new List<LigneResteaLivrer>());
            return new ClientRestesALivrerItem
            {
                ClientId = c.Client.Id,
                ClientNom = c.Client.Nom,
                Solde = c.Solde,
                LignesRestesALivrer = lignes
            };
        }).ToList();

        _logger.LogInformation("Found {Count} clients with restes à livrer", responseClients.Count);

        return Result.Ok(new RestesALivrerParClientResponse { Clients = responseClients });
    }
}
