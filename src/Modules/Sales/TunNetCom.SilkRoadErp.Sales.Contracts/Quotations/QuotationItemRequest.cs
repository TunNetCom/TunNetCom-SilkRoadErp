namespace TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;

public class QuotationItemRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("productReference")]
    public string ProductReference { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPriceExcludingTax")]
    public decimal UnitPriceExcludingTax { get; set; }

    [JsonPropertyName("discountPercentage")]
    public double DiscountPercentage { get; set; }

    [JsonPropertyName("totalExcludingTax")]
    public decimal TotalExcludingTax { get; set; }

    [JsonPropertyName("vatPercentage")]
    public double VatPercentage { get; set; }

    [JsonPropertyName("totalIncludingTax")]
    public decimal TotalIncludingTax { get; set; }
}

