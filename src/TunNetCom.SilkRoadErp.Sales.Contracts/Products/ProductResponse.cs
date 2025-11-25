namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products;

public class ProductResponse
{
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("qte")]
    public int Qte { get; set; }

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
}
