using TunNetCom.SilkRoadErp.Sales.Contracts.Common;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Responses;

public class ReceiptNoteDetailResponse : ILineItem
{
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("fournisseurId")]
    public int FournisseurId { get; set; }

    [JsonPropertyName("constructeur")]
    public bool Constructeur { get; set; }

    [JsonPropertyName("fodecPercentage")]
    public decimal FodecPercentage { get; set; }

    [JsonPropertyName("date")]
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

    [JsonPropertyName("prixHtFodec")]
    public decimal? PrixHtFodec { get; set; } // Calculated FODEC amount, nullable
}

