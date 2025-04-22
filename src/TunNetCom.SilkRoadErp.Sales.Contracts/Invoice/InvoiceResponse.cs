namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class InvoiceResponse
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("customerId")]
    public int CustomerId { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("TotalIncludingTaxAmount")]
    public decimal TotalIncludingTaxAmount { get; set; }

    [JsonPropertyName("TotalExcludingTaxAmount")]
    public decimal TotalExcludingTaxAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }
}
