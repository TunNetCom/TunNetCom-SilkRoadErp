namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class PaginatedResponse<T>
{
    public int TotalCount { get; set; }
    public List<T> Items { get; set; } = new List<T>();
}
