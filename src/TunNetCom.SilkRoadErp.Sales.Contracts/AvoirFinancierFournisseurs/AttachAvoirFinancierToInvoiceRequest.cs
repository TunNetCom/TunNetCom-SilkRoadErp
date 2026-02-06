using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

public class AttachAvoirFinancierToInvoiceRequest
{
    [JsonPropertyName("numFactureFournisseur")]
    public int NumFactureFournisseur { get; set; }
}
