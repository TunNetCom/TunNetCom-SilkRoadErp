namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

public class UpdateFactureDepenseRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("montantTotal")]
    public decimal MontantTotal { get; set; }

    [JsonPropertyName("lignesTVA")]
    public List<FactureDepenseLigneTvaDto> LignesTVA { get; set; } = new();

    [JsonPropertyName("documentBase64")]
    public string? DocumentBase64 { get; set; }
}
