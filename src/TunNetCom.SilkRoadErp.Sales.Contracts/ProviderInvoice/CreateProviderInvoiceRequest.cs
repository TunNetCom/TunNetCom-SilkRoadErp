using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class CreateProviderInvoiceRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("providerid")]
    public int ProviderId { get; set; }

    [JsonPropertyName("numFactureFournisseur")]
    public long NumFactureFournisseur { get; set; }
}

