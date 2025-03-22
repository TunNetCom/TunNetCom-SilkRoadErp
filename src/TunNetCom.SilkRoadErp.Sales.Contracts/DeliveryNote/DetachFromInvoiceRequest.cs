namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote;
public class DetachFromInvoiceRequest {
    public int InvoiceId;
    public List<int> DeliveryNoteIds;
}