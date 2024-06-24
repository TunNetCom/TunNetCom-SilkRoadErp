namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClientById;

public class GetClientByIdQueryHandler(SalesContext _context) : IRequestHandler<GetClientByIdQuery, ClientResponse>
{
    public async Task<ClientResponse> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _context.Client.FindAsync(request.Id);
        if (client == null)
        {
            throw new KeyNotFoundException($"Client with Id {request.Id} not found.");
        }

        return client.Adapt<ClientResponse>();
    }
}
