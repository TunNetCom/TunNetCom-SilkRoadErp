namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

public class GetDeliveryNotesQueryParams
{
    public int? CustomerId { get; set; }
    public int? InvoiceId { get; set; }
    public bool? IsInvoiced { get; set; } = false;
    public string? SortOrder { get; set; }
    public string? SortProperty { get; set; }
    public int? PageNumber { get; set; } 
    public int? PageSize { get; set; }
    public string? SearchKeyword { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
