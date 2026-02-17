namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

public class FactureDepenseResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("idTiersDepenseFonctionnement")]
    public int IdTiersDepenseFonctionnement { get; set; }

    [JsonPropertyName("tiersDepenseFonctionnementNom")]
    public string? TiersDepenseFonctionnementNom { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("montantTotal")]
    public decimal MontantTotal { get; set; }

    [JsonPropertyName("lignesTVA")]
    public List<FactureDepenseLigneTvaDto> LignesTVA { get; set; } = new();

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("statut")]
    public string Statut { get; set; } = string.Empty;

    [JsonPropertyName("documentStoragePath")]
    public string? DocumentStoragePath { get; set; }

    [JsonPropertyName("hasDocument")]
    public bool HasDocument { get; set; }
}
