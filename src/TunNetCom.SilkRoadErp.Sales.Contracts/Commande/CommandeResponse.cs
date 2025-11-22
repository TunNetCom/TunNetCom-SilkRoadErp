namespace TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
public class CommandeResponse
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("providerId")]
    public int ProviderId { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("TotalIncludingTaxAmount")]
    public decimal TotalIncludingTaxAmount { get; set; }

    [JsonPropertyName("TotalExcludingTaxAmount")]
    public decimal TotalExcludingTaxAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }

    [JsonPropertyName("Statut")]
    public string Statut { get; set; }
}