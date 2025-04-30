namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

public class DeliveryNoteResponse
{
    [JsonPropertyName("deliveryNoteNumber")]
    public int DeliveryNoteNumber { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("creationTime")]
    public TimeOnly CreationTime { get; set; }

    [JsonPropertyName("customerId")]
    public int? CustomerId { get; set; }

    [JsonPropertyName("invoiceNumber")]
    public int? InvoiceNumber { get; set; }

    [JsonPropertyName("totalExcludingTax")]
    public decimal TotalExcludingTax { get; set; }

    [JsonPropertyName("totalVat")]
    public decimal TotalVat { get; set; }

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("items")]
    public List<DeliveryNoteDetailResponse> Items { get; set; } = new();
}

public class DeliveryNoteDetailResponse
{
    [JsonPropertyName("Provider")]
    public string Provider { get; set; }

    [JsonPropertyName("Date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("productReference")]
    public string ProductReference { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("productReferenceAndDescription")]
    public string ProductReferenceAndDescription => $"{ProductReference} - {Description}";

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
    public decimal NetTtcUnitaire { get; set; } // Calculated
    public decimal? PrixHtFodec { get; set; } // Calculated, nullable

}