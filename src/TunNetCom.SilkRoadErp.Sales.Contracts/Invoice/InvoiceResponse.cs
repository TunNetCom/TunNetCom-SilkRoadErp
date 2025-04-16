namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class InvoiceResponse
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("customerId")]
    public int CustomerId { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("totalIncludingTax")]
    public decimal TotalIncludingTax { get; set; }

    [JsonPropertyName("totHTva")]
    public decimal TotHTva { get; set; }

    [JsonPropertyName("totTva")]
    public decimal TotTva { get; set; }
}
