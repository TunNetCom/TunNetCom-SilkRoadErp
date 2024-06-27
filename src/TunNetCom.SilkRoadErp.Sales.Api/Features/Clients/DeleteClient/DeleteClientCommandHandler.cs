namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.DeleteClient;

public class DeleteClientCommandHandler(SalesContext _context, 
    ILogger<DeleteClientCommandHandler> logger) : IRequestHandler<DeleteClientCommand, Unit>
{
    public async Task<Unit> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to delete client with ID: {Id}", request.Id);

        var client = await _context.Client.FindAsync(request.Id);
        if (client == null)
        {
            logger.LogWarning("Client with ID: {Id} not found", request.Id);
            throw new KeyNotFoundException($"Client with Id {request.Id} not found.");
        }

        _context.Client.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Client with ID: {Id} deleted successfully", request.Id);
        return Unit.Value;
    }
}
