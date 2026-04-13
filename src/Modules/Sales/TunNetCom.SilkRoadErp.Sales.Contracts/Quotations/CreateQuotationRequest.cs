namespace TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;

public class CreateQuotationRequest
{
    [JsonPropertyName("idClient")]
    public int IdClient { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("totHTva")]
    public decimal TotHTva { get; set; }

    [JsonPropertyName("TotTva")]
    public decimal TotTva { get; set; }

    [JsonPropertyName("TotTtc")]
    public decimal TotTtc { get; set; }

    [JsonPropertyName("items")]
    public List<QuotationItemRequest> Items { get; set; } = new();
}
