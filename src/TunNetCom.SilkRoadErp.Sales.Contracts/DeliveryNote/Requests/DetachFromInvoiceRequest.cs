namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;
public class DetachFromInvoiceRequest
{
    [JsonPropertyName("id")]
    public int InvoiceId { get; set; }

    [JsonPropertyName("deliveryNoteIds")]
    public required List<int> DeliveryNoteIds { get; set; } 
}