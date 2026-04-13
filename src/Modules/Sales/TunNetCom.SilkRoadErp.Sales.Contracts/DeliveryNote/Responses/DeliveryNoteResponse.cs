using TunNetCom.SilkRoadErp.Sales.Contracts.Common;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

public class DeliveryNoteResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("deliveryNoteNumber")]
    public int DeliveryNoteNumber { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("creationTime")]
    public TimeOnly CreationTime { get; set; }

    [JsonPropertyName("customerId")]
    public int? CustomerId { get; set; }

    [JsonPropertyName("installationTechnicianId")]
    public int? InstallationTechnicianId { get; set; }

    [JsonPropertyName("installationTechnicianName")]
    public string? InstallationTechnicianName { get; set; }

    [JsonPropertyName("deliveryCarId")]
    public int? DeliveryCarId { get; set; }

    [JsonPropertyName("invoiceNumber")]
    public int? InvoiceNumber { get; set; }

    [JsonPropertyName("totalExcludingTax")]
    public decimal TotalExcludingTax { get; set; }

    [JsonPropertyName("totalVat")]
    public decimal TotalVat { get; set; }

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<DeliveryNoteDetailResponse> Items { get; set; } = new();
}

public class DeliveryNoteDetailResponse : ILineItem
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

    [JsonPropertyName("deliveredQuantity")]
    public int? DeliveredQuantity { get; set; }

    [JsonPropertyName("hasPartialDelivery")]
    public bool HasPartialDelivery { get; set; }

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