namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

public class AttachToInvoiceRequest
{
    [JsonPropertyName("FactureId")]
    public int InvoiceId { get; set; }

    [JsonPropertyName("BonDeLivraisonId")]
    public List<int> DeliveryNoteIds { get; set; }
}
