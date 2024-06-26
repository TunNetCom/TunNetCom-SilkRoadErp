namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class GetClientsQuery : IRequest<PaginatedResponse<ClientResponse>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    public GetClientsQuery(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}
