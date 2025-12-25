namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class ProviderInvoiceResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("providerId")]
    public int ProviderId { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("dateFacturation")]
    public DateTime DateFacturation { get; set; }

    [JsonPropertyName("totTTC")]
    public decimal TotTTC { get; set; }

    [JsonPropertyName("totHTva")]
    public decimal TotHTva { get; set; }

    [JsonPropertyName("totTva")]
    public decimal TotTva { get; set; }

    [JsonPropertyName("providerInvoiceNumber")]
    public long ProviderInvoiceNumber { get; set; }

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;

    [JsonPropertyName("hasRetenueSource")]
    public bool HasRetenueSource { get; set; }
}
