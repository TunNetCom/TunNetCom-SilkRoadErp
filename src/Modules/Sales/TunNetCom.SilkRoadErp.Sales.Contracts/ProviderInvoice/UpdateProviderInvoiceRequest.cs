using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class UpdateProviderInvoiceRequest
{
    [JsonPropertyName("numFactureFournisseur")]
    public long NumFactureFournisseur { get; set; }
}

