namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

public class AvoirFournisseurResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("fournisseurId")]
    public int? FournisseurId { get; set; }

    [JsonPropertyName("numFactureAvoirFournisseur")]
    public int? NumFactureAvoirFournisseur { get; set; }

    [JsonPropertyName("numAvoirFournisseur")]
    public int NumAvoirFournisseur { get; set; }

    [JsonPropertyName("totalExcludingTaxAmount")]
    public decimal TotalExcludingTaxAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }

    [JsonPropertyName("totalIncludingTaxAmount")]
    public decimal TotalIncludingTaxAmount { get; set; }
}

