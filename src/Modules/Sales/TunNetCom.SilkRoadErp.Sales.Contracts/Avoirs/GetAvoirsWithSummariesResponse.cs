namespace TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

public class GetAvoirsWithSummariesResponse
{
    [JsonPropertyName("totalNetAmount")]
    public decimal TotalNetAmount { get; set; }

    [JsonPropertyName("totalVatAmount")]
    public decimal TotalVatAmount { get; set; }

    [JsonPropertyName("totalIncludingTaxAmount")]
    public decimal TotalIncludingTaxAmount { get; set; }

    /// <summary>Récap bases HT par taux (filtre courant, toutes pages).</summary>
    [JsonPropertyName("totalBaseHt7")]
    public decimal TotalBaseHt7 { get; set; }

    [JsonPropertyName("totalBaseHt13")]
    public decimal TotalBaseHt13 { get; set; }

    [JsonPropertyName("totalBaseHt19")]
    public decimal TotalBaseHt19 { get; set; }

    /// <summary>Récap montants TVA par taux.</summary>
    [JsonPropertyName("totalVat7")]
    public decimal TotalVat7 { get; set; }

    [JsonPropertyName("totalVat13")]
    public decimal TotalVat13 { get; set; }

    [JsonPropertyName("totalVat19")]
    public decimal TotalVat19 { get; set; }

    [JsonPropertyName("avoirs")]
    public PagedList<AvoirBaseInfo> Avoirs { get; set; } = new PagedList<AvoirBaseInfo>();
}

public class AvoirBaseInfo
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("clientId")]
    public int? ClientId { get; set; }

    [JsonPropertyName("clientName")]
    public string? ClientName { get; set; }

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

