using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products;

public class ProductStockResponse
{
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("stockInitial")]
    public int StockInitial { get; set; }

    [JsonPropertyName("totalAchats")]
    public int TotalAchats { get; set; }

    [JsonPropertyName("totalVentes")]
    public int TotalVentes { get; set; }

    /// <summary>
    /// Total des avoirs clients (retours clients, tous statuts)
    /// </summary>
    [JsonPropertyName("totalAvoirsClients")]
    public int TotalAvoirsClients { get; set; }

    [JsonPropertyName("stockCalcule")]
    public int StockCalcule { get; set; }

    [JsonPropertyName("stockDisponible")]
    public int StockDisponible { get; set; }

    /// <summary>
    /// Quantité totale envoyée en retour fournisseur (non encore reçue après réparation)
    /// </summary>
    [JsonPropertyName("qteEnRetourFournisseur")]
    public int QteEnRetourFournisseur { get; set; }

    /// <summary>
    /// Quantité actuellement chez le fournisseur pour réparation
    /// </summary>
    [JsonPropertyName("qteEnReparation")]
    public int QteEnReparation { get; set; }

    /// <summary>
    /// Quantité en attente de réception (réception partielle en cours)
    /// </summary>
    [JsonPropertyName("qteEnAttenteReception")]
    public int QteEnAttenteReception { get; set; }

    /// <summary>
    /// Stock réel calculé = StockCalcule - QteEnReparation
    /// </summary>
    [JsonPropertyName("stockReel")]
    public int StockReel { get; set; }
}
