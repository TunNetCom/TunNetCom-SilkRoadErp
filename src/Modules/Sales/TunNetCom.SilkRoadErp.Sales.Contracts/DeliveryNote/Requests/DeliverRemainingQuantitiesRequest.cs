namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

public class DeliverRemainingQuantitiesRequest
{
    public int InvoiceId { get; set; }
    public int? DeliveryNoteNum { get; set; }
    public List<DeliverRemainingItemRequest> Items { get; set; } = new();
}

public class DeliverRemainingItemRequest
{
    public string RefProduit { get; set; } = string.Empty;
    public string DesignationLi { get; set; } = string.Empty;
    public int QuantityToDeliver { get; set; }
}
