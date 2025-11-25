using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Services;

public class StockCalculationService : IStockCalculationService
{
    private readonly SalesContext _context;
    private readonly ILogger<StockCalculationService> _logger;

    public StockCalculationService(SalesContext context, ILogger<StockCalculationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProductStockResult> CalculateStockAsync(string refProduit, int accountingYearId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating stock for product {RefProduit} in accounting year {AccountingYearId}", refProduit, accountingYearId);

        var produit = await _context.Produit
            .FirstOrDefaultAsync(p => p.Refe == refProduit, cancellationToken);

        if (produit == null)
        {
            _logger.LogWarning("Product {RefProduit} not found", refProduit);
            return new ProductStockResult
            {
                Reference = refProduit,
                StockInitial = 0,
                TotalAchats = 0,
                TotalVentes = 0,
                StockCalcule = 0,
                StockDisponible = 0
            };
        }

        // Calculer le stock initial depuis les inventaires de l'exercice en cours
        var stockInitial = await _context.LigneInventaire
            .IgnoreQueryFilters()
            .Include(l => l.Inventaire)
            .Where(l => l.RefProduit == refProduit && l.Inventaire.AccountingYearId == accountingYearId)
            .SumAsync(l => (int?)l.QuantiteReelle, cancellationToken) ?? 0;

        // Si aucun inventaire, stock initial = 0

        // Calculer les achats (BR) pour l'exercice en cours
        var totalAchats = await _context.LigneBonReception
            .IgnoreQueryFilters()
            .Include(l => l.NumBonRecNavigation)
            .Where(l => l.RefProduit == refProduit && l.NumBonRecNavigation.AccountingYearId == accountingYearId)
            .SumAsync(l => (int?)l.QteLi, cancellationToken) ?? 0;

        // Calculer les ventes (BL) pour l'exercice en cours
        var totalVentes = await _context.LigneBl
            .IgnoreQueryFilters()
            .Include(l => l.NumBlNavigation)
            .Where(l => l.RefProduit == refProduit && l.NumBlNavigation.AccountingYearId == accountingYearId)
            .SumAsync(l => (int?)l.QteLi, cancellationToken) ?? 0;

        var stockCalcule = stockInitial + totalAchats - totalVentes;
        var stockDisponible = Math.Max(0, stockCalcule);

        _logger.LogInformation("Stock calculated for product {RefProduit}: Initial={StockInitial}, Achats={TotalAchats}, Ventes={TotalVentes}, Calculé={StockCalcule}", 
            refProduit, stockInitial, totalAchats, totalVentes, stockCalcule);

        return new ProductStockResult
        {
            Reference = refProduit,
            StockInitial = stockInitial,
            TotalAchats = totalAchats,
            TotalVentes = totalVentes,
            StockCalcule = stockCalcule,
            StockDisponible = stockDisponible
        };
    }

    public async Task<Dictionary<string, ProductStockResult>> CalculateStocksAsync(List<string> refProduits, int accountingYearId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating stocks for {Count} products in accounting year {AccountingYearId}", refProduits.Count, accountingYearId);

        var results = new Dictionary<string, ProductStockResult>();

        // Récupérer tous les produits
        var produits = await _context.Produit
            .Where(p => refProduits.Contains(p.Refe))
            .ToListAsync(cancellationToken);

        var produitDict = produits.ToDictionary(p => p.Refe, p => p);

        // Calculer le stock initial depuis les inventaires pour tous les produits
        var stocksInitiaux = await _context.LigneInventaire
            .IgnoreQueryFilters()
            .Include(l => l.Inventaire)
            .Where(l => refProduits.Contains(l.RefProduit) && l.Inventaire.AccountingYearId == accountingYearId)
            .GroupBy(l => l.RefProduit)
            .Select(g => new { RefProduit = g.Key, StockInitial = g.Sum(l => l.QuantiteReelle) })
            .ToDictionaryAsync(x => x.RefProduit, x => x.StockInitial, cancellationToken);

        // Calculer les achats (BR) pour tous les produits
        var achats = await _context.LigneBonReception
            .IgnoreQueryFilters()
            .Include(l => l.NumBonRecNavigation)
            .Where(l => refProduits.Contains(l.RefProduit) && l.NumBonRecNavigation.AccountingYearId == accountingYearId)
            .GroupBy(l => l.RefProduit)
            .Select(g => new { RefProduit = g.Key, TotalAchats = g.Sum(l => l.QteLi) })
            .ToDictionaryAsync(x => x.RefProduit, x => x.TotalAchats, cancellationToken);

        // Calculer les ventes (BL) pour tous les produits
        var ventes = await _context.LigneBl
            .IgnoreQueryFilters()
            .Include(l => l.NumBlNavigation)
            .Where(l => refProduits.Contains(l.RefProduit) && l.NumBlNavigation.AccountingYearId == accountingYearId)
            .GroupBy(l => l.RefProduit)
            .Select(g => new { RefProduit = g.Key, TotalVentes = g.Sum(l => l.QteLi) })
            .ToDictionaryAsync(x => x.RefProduit, x => x.TotalVentes, cancellationToken);

        // Construire les résultats
        foreach (var refProduit in refProduits)
        {
            var stockInitial = stocksInitiaux.GetValueOrDefault(refProduit, 0);
            
            // Si aucun inventaire, stock initial = 0

            var totalAchats = achats.GetValueOrDefault(refProduit, 0);
            var totalVentes = ventes.GetValueOrDefault(refProduit, 0);
            var stockCalcule = stockInitial + totalAchats - totalVentes;
            var stockDisponible = Math.Max(0, stockCalcule);

            results[refProduit] = new ProductStockResult
            {
                Reference = refProduit,
                StockInitial = stockInitial,
                TotalAchats = totalAchats,
                TotalVentes = totalVentes,
                StockCalcule = stockCalcule,
                StockDisponible = stockDisponible
            };
        }

        _logger.LogInformation("Stocks calculated for {Count} products", results.Count);
        return results;
    }
}

