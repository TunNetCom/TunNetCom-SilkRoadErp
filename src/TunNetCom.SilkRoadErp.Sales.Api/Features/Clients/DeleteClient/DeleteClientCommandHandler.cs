namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.DeleteClient;

public class DeleteClientCommandHandler(SalesContext _context) : IRequestHandler<DeleteClientCommand, Unit>
{
    public async Task<Unit> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Client.FindAsync(request.Id);
        if (client == null)
        {
            
            throw new KeyNotFoundException($"Client with Id {request.Id} not found.");
        }

        _context.Client.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
