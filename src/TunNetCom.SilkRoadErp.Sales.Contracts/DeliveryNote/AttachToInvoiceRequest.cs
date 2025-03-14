using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote;

public class AttachToInvoiceRequest
{
    [JsonPropertyName("FactureId")]
    public int InvoiceId { get; set; }

    [JsonPropertyName("BonDeLivraisonId")]
    public List<int> DeliveryNoteIds { get; set; }
}
