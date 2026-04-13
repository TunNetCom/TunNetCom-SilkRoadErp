namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products;

public class ProductResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("qteLimit")]
    public int QteLimit { get; set; }

    [JsonPropertyName("discountPourcentage")]
    public double DiscountPourcentage { get; set; }

    [JsonPropertyName("discountPourcentageOfPurchasing")]
    public double DiscountPourcentageOfPurchasing { get; set; }

    [JsonPropertyName("vatRate")]
    public double VatRate { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("purchasingPrice")]
    public decimal PurchasingPrice { get; set; }

    [JsonPropertyName("visibility")]
    public bool Visibility { get; set; }

    [JsonPropertyName("stockCalcule")]
    public int? StockCalcule { get; set; }

    [JsonPropertyName("stockDisponible")]
    public int? StockDisponible { get; set; }

    [JsonPropertyName("isStockLow")]
    public bool IsStockLow { get; set; }

    /// <summary>
    /// Quantité en retour fournisseur (chez le fournisseur pour réparation)
    /// </summary>
    [JsonPropertyName("qteEnRetourFournisseur")]
    public int? QteEnRetourFournisseur { get; set; }

    /// <summary>
    /// Quantité actuellement en réparation chez le fournisseur
    /// </summary>
    [JsonPropertyName("qteEnReparation")]
    public int? QteEnReparation { get; set; }

    /// <summary>
    /// Stock réel (stock calculé - quantité en réparation)
    /// </summary>
    [JsonPropertyName("stockReel")]
    public int? StockReel { get; set; }

    [JsonPropertyName("sousFamilleProduitId")]
    public int? SousFamilleProduitId { get; set; }

    [JsonPropertyName("sousFamilleProduitNom")]
    public string? SousFamilleProduitNom { get; set; }

    [JsonPropertyName("image1StoragePath")]
    public string? Image1StoragePath { get; set; }

    [JsonPropertyName("image2StoragePath")]
    public string? Image2StoragePath { get; set; }

    [JsonPropertyName("image3StoragePath")]
    public string? Image3StoragePath { get; set; }
}
