namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

public class CreateAvoirFinancierFournisseursRequest
{
    [JsonPropertyName("numFactureFournisseur")]
    public int NumFactureFournisseur { get; set; }

    [JsonPropertyName("numSurPage")]
    public int NumSurPage { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("totTtc")]
    public decimal TotTtc { get; set; }
}

