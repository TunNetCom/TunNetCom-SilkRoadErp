namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public record GetClientsQuery(
    int PageIndex,
    int PageSize,
    string? SearchKeyword) : IRequest<PagedList<ClientResponse>>;
