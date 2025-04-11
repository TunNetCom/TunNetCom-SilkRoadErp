namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class ReceiptNotesQuery
{
    public int pageNumber { get; set; }
    public int pageSize { get; set; }
    public string? sortProperty { get; set; }
    public string? sortOrder { get; set; }
    public string? searchKeyword { get; set; }
    public int providerId { get; set; }
    public bool isInvoiced { get; set; }
    public int? invoiceId { get; set; }
}