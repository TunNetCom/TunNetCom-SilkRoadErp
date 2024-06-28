namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class GetClientsQueryHandler(SalesContext _context, ILogger<GetClientsQueryHandler> logger)
    : IRequestHandler<GetClientsQuery, PaginatedResponse<ClientResponse>>
{
    public async Task<PaginatedResponse<ClientResponse>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching clients with pageIndex: {PageIndex} and pageSize: {PageSize}", request.PageIndex, request.PageSize);

        var totalClients = await _context.Client.CountAsync(cancellationToken);
        var clients = await _context.Client
                                    .Skip((request.PageIndex - 1) * request.PageSize)
                                    .Take(request.PageSize)
                                    .ToListAsync(cancellationToken);

        var clientResponses = clients.Adapt<List<ClientResponse>>();

        logger.LogInformation("Fetched {Count} clients", clientResponses.Count);

        return new PaginatedResponse<ClientResponse>
        {
            TotalCount = totalClients,
            Items = clientResponses
        };
    }
}
