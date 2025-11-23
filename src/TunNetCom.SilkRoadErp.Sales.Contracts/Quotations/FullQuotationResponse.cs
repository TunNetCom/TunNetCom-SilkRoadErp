namespace TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;

public class FullQuotationResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("customerId")]
    public int CustomerId { get; set; }

    [JsonPropertyName("totalExcludingTax")]
    public decimal TotalExcludingTax { get; set; }

    [JsonPropertyName("totalVat")]
    public decimal TotalVat { get; set; }

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("items")]
    public List<QuotationDetailResponse> Items { get; set; } = new();
}

