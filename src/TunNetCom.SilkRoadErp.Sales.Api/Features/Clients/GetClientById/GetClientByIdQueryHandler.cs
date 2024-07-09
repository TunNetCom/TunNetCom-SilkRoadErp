using Azure.Core;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClientById;

public class GetClientByIdQueryHandler(SalesContext _context, ILogger<GetClientByIdQueryHandler> _logger)
    : IRequestHandler<GetClientByIdQuery, Result<ClientResponse>>
{
    public async Task<Result<ClientResponse>> Handle(GetClientByIdQuery getClientByIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingClientById(
            getClientByIdQuery.Id);

        var client = await _context.Client.FindAsync(getClientByIdQuery.Id);
        if (client is null)
        {
            _logger.LogClientNotFound(
                getClientByIdQuery.Id);

            return Result.Fail("client_not_found");
        }

        _logger.LogClientFetchedById(
            getClientByIdQuery.Id);
        return client.Adapt<ClientResponse>();
    }
}
