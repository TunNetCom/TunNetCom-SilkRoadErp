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
                TotalAvoirsClients = 0,
                StockCalcule = 0,
                StockDisponible = 0,
                QteEnRetourFournisseur = 0,
                QteEnReparation = 0,
                QteEnAttenteReception = 0,
                StockReel = 0
            };
        }

        // Calculer le stock initial depuis les inventaires de l'exercice en cours
        var stockInitial = await _context.LigneInventaire
            .IgnoreQueryFilters()
            .Include(l => l.Inventaire)
            .Where(l => l.RefProduit == refProduit && l.Inventaire.AccountingYearId == accountingYearId && (l.Inventaire.Statut == InventaireStatut.Valide || l.Inventaire.Statut == InventaireStatut.Cloture))
            .SumAsync(l => (int?)l.QuantiteReelle, cancellationToken) ?? 0;

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

        // Calculer les avoirs clients (retours clients) pour l'exercice en cours (tous statuts)
        var totalAvoirsClients = await _context.LigneAvoirs
            .IgnoreQueryFilters()
            .Include(l => l.AvoirsNavigation)
            .Where(l => l.RefProduit == refProduit && l.AvoirsNavigation.AccountingYearId == accountingYearId)
            .SumAsync(l => (int?)l.QteLi, cancellationToken) ?? 0;

        // Calculer les quantités en retour fournisseur
        var retourFournisseurData = await CalculateRetourFournisseurDataAsync(refProduit, accountingYearId, cancellationToken);

        var stockCalcule = stockInitial + totalAchats - totalVentes + totalAvoirsClients;
        
        // Le stock disponible exclut les produits en retour fournisseur (non encore reçus)
        var stockDisponible = Math.Max(0, stockCalcule - retourFournisseurData.QteEnReparation);
        
        // Stock réel = stock calculé - quantités chez le fournisseur
        var stockReel = stockCalcule - retourFournisseurData.QteEnReparation;

        _logger.LogInformation(
            "Stock calculated for product {RefProduit}: Initial={StockInitial}, Achats={TotalAchats}, Ventes={TotalVentes}, AvoirsClients={TotalAvoirsClients}, Calculé={StockCalcule}, EnReparation={EnReparation}, Disponible={Disponible}",
            refProduit, stockInitial, totalAchats, totalVentes, totalAvoirsClients, stockCalcule, retourFournisseurData.QteEnReparation, stockDisponible);

        return new ProductStockResult
        {
            Reference = refProduit,
            StockInitial = stockInitial,
            TotalAchats = totalAchats,
            TotalVentes = totalVentes,
            TotalAvoirsClients = totalAvoirsClients,
            StockCalcule = stockCalcule,
            StockDisponible = stockDisponible,
            QteEnRetourFournisseur = retourFournisseurData.QteEnRetourFournisseur,
            QteEnReparation = retourFournisseurData.QteEnReparation,
            QteEnAttenteReception = retourFournisseurData.QteEnAttenteReception,
            StockReel = stockReel
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
            .Where(l => refProduits.Contains(l.RefProduit) && l.Inventaire.AccountingYearId == accountingYearId && (l.Inventaire.Statut == InventaireStatut.Valide || l.Inventaire.Statut == InventaireStatut.Cloture))
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

        // Calculer les avoirs clients (retours clients) pour tous les produits (tous statuts)
        var avoirsClients = await _context.LigneAvoirs
            .IgnoreQueryFilters()
            .Include(l => l.AvoirsNavigation)
            .Where(l => refProduits.Contains(l.RefProduit) && l.AvoirsNavigation.AccountingYearId == accountingYearId)
            .GroupBy(l => l.RefProduit)
            .Select(g => new { RefProduit = g.Key, TotalAvoirsClients = g.Sum(l => l.QteLi) })
            .ToDictionaryAsync(x => x.RefProduit, x => x.TotalAvoirsClients, cancellationToken);

        // Calculer les retours fournisseur pour tous les produits
        var retoursFournisseur = await CalculateRetourFournisseurDataBatchAsync(refProduits, accountingYearId, cancellationToken);

        // Construire les résultats
        foreach (var refProduit in refProduits)
        {
            var stockInitial = stocksInitiaux.GetValueOrDefault(refProduit, 0);
            var totalAchats = achats.GetValueOrDefault(refProduit, 0);
            var totalVentes = ventes.GetValueOrDefault(refProduit, 0);
            var totalAvoirsClients = avoirsClients.GetValueOrDefault(refProduit, 0);
            var stockCalcule = stockInitial + totalAchats - totalVentes + totalAvoirsClients;
            
            var retourData = retoursFournisseur.GetValueOrDefault(refProduit, new RetourFournisseurData());
            
            // Le stock disponible exclut les produits en retour fournisseur
            var stockDisponible = Math.Max(0, stockCalcule - retourData.QteEnReparation);
            var stockReel = stockCalcule - retourData.QteEnReparation;

            results[refProduit] = new ProductStockResult
            {
                Reference = refProduit,
                StockInitial = stockInitial,
                TotalAchats = totalAchats,
                TotalVentes = totalVentes,
                TotalAvoirsClients = totalAvoirsClients,
                StockCalcule = stockCalcule,
                StockDisponible = stockDisponible,
                QteEnRetourFournisseur = retourData.QteEnRetourFournisseur,
                QteEnReparation = retourData.QteEnReparation,
                QteEnAttenteReception = retourData.QteEnAttenteReception,
                StockReel = stockReel
            };
        }

        _logger.LogInformation("Stocks calculated for {Count} products", results.Count);
        return results;
    }

    /// <summary>
    /// Calcule les données de retour fournisseur pour un seul produit
    /// </summary>
    private async Task<RetourFournisseurData> CalculateRetourFournisseurDataAsync(
        string refProduit, 
        int accountingYearId, 
        CancellationToken cancellationToken)
    {
        // Statuts où le produit est considéré "en retour" (hors brouillon et clôturé)
        var statutsEnRetour = new[] 
        { 
            RetourFournisseurStatus.Valid, 
            RetourFournisseurStatus.EnReparation, 
            RetourFournisseurStatus.ReceptionPartielle 
        };

        var lignesRetour = await _context.LigneRetourMarchandiseFournisseur
            .IgnoreQueryFilters()
            .Include(l => l.RetourMarchandiseFournisseurNavigation)
            .Where(l => l.RefProduit == refProduit 
                && l.RetourMarchandiseFournisseurNavigation.AccountingYearId == accountingYearId
                && statutsEnRetour.Contains(l.RetourMarchandiseFournisseurNavigation.StatutRetour))
            .Select(l => new
            {
                QteLi = l.QteLi,
                QteRecue = l.QteRecue,
                Statut = l.RetourMarchandiseFournisseurNavigation.StatutRetour
            })
            .ToListAsync(cancellationToken);

        // Quantité totale envoyée en retour fournisseur
        var qteEnRetourFournisseur = lignesRetour.Sum(l => l.QteLi);
        
        // Quantité actuellement chez le fournisseur (envoyée - reçue)
        var qteEnReparation = lignesRetour.Sum(l => Math.Max(0, l.QteLi - l.QteRecue));
        
        // Quantité en attente de réception (pour les retours en réception partielle)
        var qteEnAttenteReception = lignesRetour
            .Where(l => l.Statut == RetourFournisseurStatus.ReceptionPartielle)
            .Sum(l => Math.Max(0, l.QteLi - l.QteRecue));

        return new RetourFournisseurData
        {
            QteEnRetourFournisseur = qteEnRetourFournisseur,
            QteEnReparation = qteEnReparation,
            QteEnAttenteReception = qteEnAttenteReception
        };
    }

    /// <summary>
    /// Calcule les données de retour fournisseur pour plusieurs produits en batch
    /// </summary>
    private async Task<Dictionary<string, RetourFournisseurData>> CalculateRetourFournisseurDataBatchAsync(
        List<string> refProduits, 
        int accountingYearId, 
        CancellationToken cancellationToken)
    {
        var statutsEnRetour = new[] 
        { 
            RetourFournisseurStatus.Valid, 
            RetourFournisseurStatus.EnReparation, 
            RetourFournisseurStatus.ReceptionPartielle 
        };

        var lignesRetour = await _context.LigneRetourMarchandiseFournisseur
            .IgnoreQueryFilters()
            .Include(l => l.RetourMarchandiseFournisseurNavigation)
            .Where(l => refProduits.Contains(l.RefProduit) 
                && l.RetourMarchandiseFournisseurNavigation.AccountingYearId == accountingYearId
                && statutsEnRetour.Contains(l.RetourMarchandiseFournisseurNavigation.StatutRetour))
            .Select(l => new
            {
                RefProduit = l.RefProduit,
                QteLi = l.QteLi,
                QteRecue = l.QteRecue,
                Statut = l.RetourMarchandiseFournisseurNavigation.StatutRetour
            })
            .ToListAsync(cancellationToken);

        return lignesRetour
            .GroupBy(l => l.RefProduit)
            .ToDictionary(
                g => g.Key,
                g => new RetourFournisseurData
                {
                    QteEnRetourFournisseur = g.Sum(l => l.QteLi),
                    QteEnReparation = g.Sum(l => Math.Max(0, l.QteLi - l.QteRecue)),
                    QteEnAttenteReception = g
                        .Where(l => l.Statut == RetourFournisseurStatus.ReceptionPartielle)
                        .Sum(l => Math.Max(0, l.QteLi - l.QteRecue))
                });
    }

    /// <summary>
    /// Classe interne pour stocker les données de retour fournisseur
    /// </summary>
    private class RetourFournisseurData
    {
        public int QteEnRetourFournisseur { get; set; }
        public int QteEnReparation { get; set; }
        public int QteEnAttenteReception { get; set; }
    }
}
