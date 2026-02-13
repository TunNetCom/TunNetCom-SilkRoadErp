namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

public class GetFacturesDepenseWithSummariesResponse
{
    [JsonPropertyName("items")]
    public List<FactureDepenseSummaryItem> Items { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}

public class FactureDepenseSummaryItem
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

    [JsonPropertyName("statut")]
    public string Statut { get; set; } = string.Empty;

    [JsonPropertyName("hasDocument")]
    public bool HasDocument { get; set; }

    [JsonPropertyName("exonereRetenueSource")]
    public bool ExonereRetenueSource { get; set; }

    [JsonPropertyName("hasRetenueSource")]
    public bool HasRetenueSource { get; set; }
}
