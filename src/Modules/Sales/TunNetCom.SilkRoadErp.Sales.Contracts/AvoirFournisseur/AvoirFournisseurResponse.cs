namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

public class AvoirFournisseurResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("fournisseurId")]
    public int? FournisseurId { get; set; }

    [JsonPropertyName("numFactureAvoirFournisseur")]
    public int? NumFactureAvoirFournisseur { get; set; }

    [JsonPropertyName("numAvoirChezFournisseur")]
    public int NumAvoirChezFournisseur { get; set; }

    [JsonPropertyName("totalExcludingTaxAmount")]
    public decimal TotalExcludingTaxAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }

    [JsonPropertyName("totalIncludingTaxAmount")]
    public decimal TotalIncludingTaxAmount { get; set; }

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;
}

