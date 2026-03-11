namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class AttachReceiptNotesRequest
{
    [JsonPropertyName("invoiceId")]
    public int InvoiceId { get; set; }

    [JsonPropertyName("receiptNotesIds")]
    public required List<int> ReceiptNotesIds { get; set; }
}
