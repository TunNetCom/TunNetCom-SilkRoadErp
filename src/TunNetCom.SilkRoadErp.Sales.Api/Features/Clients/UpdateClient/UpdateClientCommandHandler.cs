namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientCommandHandler(
    SalesContext _context,
    ILogger<UpdateClientCommandHandler> _logger)
    : IRequestHandler<UpdateClientCommand, Result>
{
    public async Task<Result> Handle(UpdateClientCommand updateClientCommand, CancellationToken cancellationToken)
    {
        //TODO Back : High-performance logging with LoggerMessage #19
        _logger.LogClientUpdateAttempt(
            updateClientCommand.Id);

        var clientToUpdate = await _context.Client.FindAsync(updateClientCommand.Id);
            
        if (clientToUpdate is null)
        {
            _logger.LogClientNotFound(
                updateClientCommand.Id);

            return Result.Fail("Client not found.");
        }

        clientToUpdate.UpdateClient(
            nom: updateClientCommand.Nom,
            tel: updateClientCommand.Tel,
            adresse: updateClientCommand.Adresse,
            matricule: updateClientCommand.Matricule,
            code: updateClientCommand.Code,
            codeCat: updateClientCommand.CodeCat,
            etbSec: updateClientCommand.EtbSec,
            mail: updateClientCommand.Mail);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogClientUpdated(
            updateClientCommand.Id);

        return Result.Ok();
    }
}
