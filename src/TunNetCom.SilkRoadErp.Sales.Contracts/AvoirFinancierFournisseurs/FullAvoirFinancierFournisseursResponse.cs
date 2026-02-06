namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

public class FullAvoirFinancierFournisseursResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("numSurPage")]
    public int NumSurPage { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("totTtc")]
    public decimal TotTtc { get; set; }

    [JsonPropertyName("factureFournisseur")]
    public AvoirFinancierFournisseursFactureFournisseurResponse? FactureFournisseur { get; set; }
}

public class AvoirFinancierFournisseursFactureFournisseurResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("providerId")]
    public int ProviderId { get; set; }

    [JsonPropertyName("providerName")]
    public string ProviderName { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("dateFacturation")]
    public DateTime DateFacturation { get; set; }

    [JsonPropertyName("providerInvoiceNumber")]
    public long ProviderInvoiceNumber { get; set; }

    [JsonPropertyName("totTTC")]
    public decimal TotTTC { get; set; }
}

