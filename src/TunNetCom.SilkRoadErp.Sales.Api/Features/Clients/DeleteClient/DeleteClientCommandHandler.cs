namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.DeleteClient;

public class DeleteClientCommandHandler(SalesContext _context, 
    ILogger<DeleteClientCommandHandler> _logger) : IRequestHandler<DeleteClientCommand, Result>
{
    public async Task<Result> Handle(DeleteClientCommand deleteClientCommand, CancellationToken cancellationToken)
    {
        Log.DeletingClient(
            _logger,
            deleteClientCommand.Id);

        var client = await _context.Client.FindAsync(deleteClientCommand.Id);

        if (client is null)
        {
            Log.ClientNotFound(
                _logger,
                deleteClientCommand.Id);

            return Result.Fail("client_not_found");
        }

        _context.Client.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);

        Log.ClientDeleted(
            _logger,
            deleteClientCommand.Id);

        return Result.Ok();
    }
}
