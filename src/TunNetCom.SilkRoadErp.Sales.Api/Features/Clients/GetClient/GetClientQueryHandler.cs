namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class GetClientsQueryHandler(SalesContext _context, ILogger<GetClientsQueryHandler> logger)
    : IRequestHandler<GetClientsQuery, PagedList<ClientResponse>>
{
    public async Task<PagedList<ClientResponse>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        //TODO Back : High-performance logging with LoggerMessage #19

        logger.LogInformation(
            "Fetching clients with pageIndex: {PageIndex} and pageSize: {PageSize}",
            request.PageIndex,
            request.PageSize);

        var clientsQuery = _context.Client.AsQueryable();

        clientsQuery = clientsQuery.Where(
            c =>
            c.Nom.Contains(request.SearchKeyword)
            || c.Mail.Contains(request.SearchKeyword));

        var pagedClients = await PagedList<Client>.ToPagedListAsync(clientsQuery, request.PageIndex, request.PageSize, cancellationToken);

        var clientResponses = pagedClients.Select(c => c.Adapt<ClientResponse>()).ToList();

        logger.LogInformation("Fetched {Count} clients", clientResponses.Count);

        return new PagedList<ClientResponse>(clientResponses, pagedClients.TotalCount, pagedClients.CurrentPage, pagedClients.PageSize);
    }
}
