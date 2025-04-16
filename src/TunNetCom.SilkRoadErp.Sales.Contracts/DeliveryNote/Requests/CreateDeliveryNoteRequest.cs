namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;
using System.Text.Json.Serialization;

public class CreateDeliveryNoteRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("totalExcludingTax")]
    public decimal TotalExcludingTax { get; set; }

    [JsonPropertyName("totalVat")]
    public decimal TotalVat { get; set; }

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("deliveryTime")]
    public TimeOnly DeliveryTime { get; set; }

    [JsonPropertyName("invoiceNumber")]
    public int? InvoiceNumber { get; set; }

    [JsonPropertyName("customerId")]
    public int? CustomerId { get; set; }

    [JsonPropertyName("items")]
    public List<DeliveryNoteItemRequest> Items { get; set; } = new();
}

public class DeliveryNoteItemRequest
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
