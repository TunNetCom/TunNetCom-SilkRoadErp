namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

public class GetAvoirFinancierFournisseursWithSummariesResponse
{
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    [JsonPropertyName("avoirFinancierFournisseurs")]
    public PagedList<AvoirFinancierFournisseursBaseInfo> AvoirFinancierFournisseurs { get; set; } = new PagedList<AvoirFinancierFournisseursBaseInfo>();
}

public class AvoirFinancierFournisseursBaseInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("numSurPage")]
    public int NumSurPage { get; set; }

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("totTtc")]
    public decimal TotTtc { get; set; }

    [JsonPropertyName("numFactureFournisseur")]
    public int NumFactureFournisseur { get; set; }

    [JsonPropertyName("providerId")]
    public int ProviderId { get; set; }

    [JsonPropertyName("providerName")]
    public string? ProviderName { get; set; }

    [JsonPropertyName("providerInvoiceNumber")]
    public long ProviderInvoiceNumber { get; set; }
}

