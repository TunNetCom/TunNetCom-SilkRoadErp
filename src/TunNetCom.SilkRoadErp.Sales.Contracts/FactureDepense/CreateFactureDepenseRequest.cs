namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

public class CreateFactureDepenseRequest
{
    [JsonPropertyName("idTiersDepenseFonctionnement")]
    public int IdTiersDepenseFonctionnement { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("montantTotal")]
    public decimal MontantTotal { get; set; }

    [JsonPropertyName("accountingYearId")]
    public int? AccountingYearId { get; set; }

    [JsonPropertyName("documentBase64")]
    public string? DocumentBase64 { get; set; }
}
