namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClientById;

public class GetClientByIdQueryHandler(SalesContext _context, ILogger<GetClientByIdQueryHandler> logger)
    : IRequestHandler<GetClientByIdQuery, ClientResponse>
{
    public async Task<ClientResponse> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching client with ID: {Id}", request.Id);

        var client = await _context.Client.FindAsync(request.Id);
        if (client == null)
        {
            logger.LogWarning("Client with ID: {Id} not found", request.Id);
            throw new KeyNotFoundException($"Client with Id {request.Id} not found.");
        }

        logger.LogInformation("Fetched client with ID: {Id}", request.Id);
        return client.Adapt<ClientResponse>();
    }
}
