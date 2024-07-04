namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClientById;

public class GetClientByIdQueryHandler(SalesContext _context, ILogger<GetClientByIdQueryHandler> logger)
    : IRequestHandler<GetClientByIdQuery, Result<ClientResponse>>
{
    public async Task<Result<ClientResponse>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching client with ID: {Id}", request.Id);

        var client = await _context.Client.FindAsync(request.Id);
        if (client is null)
        {
            logger.LogWarning("Client with ID: {Id} not found", request.Id);

            return Result.Fail("client_not_found");
        }

        logger.LogInformation("Fetched client with ID: {Id}", request.Id);
        return client.Adapt<ClientResponse>();
    }
}
