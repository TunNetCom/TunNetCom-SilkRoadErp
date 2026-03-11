namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

public class GetFactureAvoirFournisseurWithSummariesResponse
{
    [JsonPropertyName("totalNetAmount")]
    public decimal TotalNetAmount { get; set; }

    [JsonPropertyName("totalVatAmount")]
    public decimal TotalVatAmount { get; set; }

    [JsonPropertyName("totalIncludingTaxAmount")]
    public decimal TotalIncludingTaxAmount { get; set; }

    [JsonPropertyName("factureAvoirFournisseurs")]
    public PagedList<FactureAvoirFournisseurBaseInfo> FactureAvoirFournisseurs { get; set; } = new PagedList<FactureAvoirFournisseurBaseInfo>();
}

public class FactureAvoirFournisseurBaseInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numFactureAvoirFourSurPage")]
    public int NumFactureAvoirFourSurPage { get; set; }

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("idFournisseur")]
    public int IdFournisseur { get; set; }

    [JsonPropertyName("fournisseurName")]
    public string? FournisseurName { get; set; }

    [JsonPropertyName("numFactureFournisseur")]
    public int? NumFactureFournisseur { get; set; }

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("accountingYearName")]
    public string AccountingYearName { get; set; } = string.Empty;

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

