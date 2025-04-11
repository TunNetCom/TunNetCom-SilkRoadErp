namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class DetachReceiptNotesRequest
{
    public List<int> ReceiptNoteIds { get; set; } = new List<int>();
    public int InvoiceId { get; set; }
}
