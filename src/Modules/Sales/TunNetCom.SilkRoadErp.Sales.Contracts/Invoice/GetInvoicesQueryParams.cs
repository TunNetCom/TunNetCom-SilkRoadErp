namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class GetInvoicesQueryParams
{
    public int? CustomerId { get; set; }
    public string? SortOrder { get; set; }
    public string? SortProperty { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? SearchKeyword { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

