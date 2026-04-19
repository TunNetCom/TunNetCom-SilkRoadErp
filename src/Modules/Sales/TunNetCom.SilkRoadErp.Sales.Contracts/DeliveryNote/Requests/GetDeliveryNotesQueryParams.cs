namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

public class GetDeliveryNotesQueryParams
{
    public int? CustomerId { get; set; }
    public int? InvoiceId { get; set; }
    /// <summary>When null, do not filter by invoice attachment (all delivery notes).</summary>
    public bool? IsInvoiced { get; set; }
    public string? SortOrder { get; set; }
    public string? SortProperty { get; set; }
    public int? PageNumber { get; set; } 
    public int? PageSize { get; set; }
    public string? SearchKeyword { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Status { get; set; }
    public int? TechnicianId { get; set; }
    // TagIds is extracted as a separate parameter in the endpoint to avoid body inference
}
