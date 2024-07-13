namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.DeleteClient;

public class DeleteClientCommandHandler(SalesContext _context, 
    ILogger<DeleteClientCommandHandler> _logger) : IRequestHandler<DeleteClientCommand, Result>
{
    public async Task<Result> Handle(DeleteClientCommand deleteClientCommand, CancellationToken cancellationToken)
    {
        _logger.LogClientDeletionAttempt(
            deleteClientCommand.Id);

        var client = await _context.Client.FindAsync(deleteClientCommand.Id);

        if (client is null)
        {
            _logger.LogClientNotFound(
                deleteClientCommand.Id);

            return Result.Fail("client_not_found");
        }

        _context.Client.Remove(client);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogClientDeleted(
            deleteClientCommand.Id);

        return Result.Ok();
    }
}
