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

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;
}