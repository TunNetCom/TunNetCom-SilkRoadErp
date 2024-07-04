namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.DeleteClient;

public class DeleteClientCommandHandler(SalesContext _context, 
    ILogger<DeleteClientCommandHandler> logger) : IRequestHandler<DeleteClientCommand, Result>
{
    public async Task<Result> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to delete client with ID: {Id}", request.Id);

        var client = await _context.Client.FindAsync(request.Id);

        if (client is null)
        {
            logger.LogWarning("Client with ID: {Id} not found", request.Id);
            return Result.Fail("client_not_found");
        }

        _context.Client.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Client with ID: {Id} deleted successfully", request.Id);

        return Result.Ok();
    }
}
