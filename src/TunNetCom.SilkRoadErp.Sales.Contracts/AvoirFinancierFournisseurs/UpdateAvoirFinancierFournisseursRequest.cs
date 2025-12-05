namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

public class UpdateAvoirFinancierFournisseursRequest
{
    [JsonPropertyName("numSurPage")]
    public int NumSurPage { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("totTtc")]
    public decimal TotTtc { get; set; }
}

