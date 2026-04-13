namespace TunNetCom.SilkRoadErp.Sales.Domain.Services;

public interface IStockCalculationService
{
    Task<ProductStockResult> CalculateStockAsync(string refProduit, int accountingYearId, CancellationToken cancellationToken = default);
    Task<Dictionary<string, ProductStockResult>> CalculateStocksAsync(List<string> refProduits, int accountingYearId, CancellationToken cancellationToken = default);
}

public class ProductStockResult
{
    public string Reference { get; set; } = string.Empty;
    
    /// <summary>
    /// Stock initial depuis l'inventaire
    /// </summary>
    public int StockInitial { get; set; }
    
    /// <summary>
    /// Total des achats (bons de réception)
    /// </summary>
    public int TotalAchats { get; set; }
    
    /// <summary>
    /// Total des ventes (bons de livraison)
    /// </summary>
    public int TotalVentes { get; set; }
    
    /// <summary>
    /// Total des avoirs clients (retours clients, tous statuts)
    /// </summary>
    public int TotalAvoirsClients { get; set; }
    
    /// <summary>
    /// Stock calculé = StockInitial + TotalAchats - TotalVentes + TotalAvoirsClients
    /// </summary>
    public int StockCalcule { get; set; }
    
    /// <summary>
    /// Stock disponible (max(0, StockCalcule - QteEnRetourFournisseur))
    /// </summary>
    public int StockDisponible { get; set; }
    
    /// <summary>
    /// Quantité totale envoyée en retour fournisseur (non encore reçue après réparation)
    /// </summary>
    public int QteEnRetourFournisseur { get; set; }
    
    /// <summary>
    /// Quantité actuellement chez le fournisseur pour réparation
    /// </summary>
    public int QteEnReparation { get; set; }
    
    /// <summary>
    /// Quantité en attente de réception (réception partielle en cours)
    /// </summary>
    public int QteEnAttenteReception { get; set; }
    
    /// <summary>
    /// Stock réel calculé = StockCalcule - QteEnReparation
    /// </summary>
    public int StockReel { get; set; }
}
