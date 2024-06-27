namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class PaginatedResponse<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public List<T> Items { get; set; } = new List<T>();
}

