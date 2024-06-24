namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class GetClientsQueryHandler(SalesContext _context) : IRequestHandler<GetClientsQuery, PaginatedResponse<ClientResponse>>
{
    public async Task<PaginatedResponse<ClientResponse>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var totalClients = await _context.Client.CountAsync(cancellationToken);
        var clients = await _context.Client
                                    .Skip((request.PageIndex - 1) * request.PageSize)
                                    .Take(request.PageSize)
                                    .ToListAsync(cancellationToken);

        var clientResponses = clients.Adapt<List<ClientResponse>>();

        return new PaginatedResponse<ClientResponse>
        {
            TotalCount = totalClients,
            Items = clientResponses
        };
    }
}
