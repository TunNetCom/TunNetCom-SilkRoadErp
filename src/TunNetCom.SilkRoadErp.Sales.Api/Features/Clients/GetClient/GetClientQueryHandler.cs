namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

public class GetClientsQueryHandler(SalesContext _context, ILogger<GetClientsQueryHandler> _logger)
    : IRequestHandler<GetClientsQuery, PagedList<ClientResponse>>
{
    public async Task<PagedList<ClientResponse>> Handle(GetClientsQuery getClientsQuery, CancellationToken cancellationToken)
    {
        //TODO Back : High-performance logging with LoggerMessage #19

       Log.FetchingClients(
           _logger,
           getClientsQuery.PageNumber,
           getClientsQuery.PageSize);

        var clientsQuery = _context.Client.AsQueryable();

        if (!string.IsNullOrEmpty(getClientsQuery.SearchKeyword))
        {
            clientsQuery = clientsQuery.Where(
                c => c.Nom.Contains(getClientsQuery.SearchKeyword) || 
                c.Tel.Contains(getClientsQuery.SearchKeyword) ||
                c.Adresse.Contains(getClientsQuery.SearchKeyword) ||
                c.Matricule.Contains(getClientsQuery.SearchKeyword) ||
                c.Code.Contains(getClientsQuery.SearchKeyword) ||
                c.CodeCat.Contains(getClientsQuery.SearchKeyword) ||
                c.EtbSec.Contains(getClientsQuery.SearchKeyword) ||
                c.Mail.Contains(getClientsQuery.SearchKeyword)) ;
        }

        var pagedClients = await PagedList<Client>.ToPagedListAsync(clientsQuery, getClientsQuery.PageNumber, getClientsQuery.PageSize, cancellationToken);

        var clientResponses = pagedClients.Select(c => c.Adapt<ClientResponse>()).ToList();

        Log.FetchedClients(
            _logger,
            clientResponses.Count);

        return new PagedList<ClientResponse>(clientResponses, pagedClients.TotalCount, pagedClients.CurrentPage, pagedClients.PageSize);
    }
}
