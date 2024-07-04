namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientCommandHandler(
    SalesContext _context,
    ILogger<UpdateClientCommandHandler> logger)
    : IRequestHandler<UpdateClientCommand, Result>
{
    public async Task<Result> Handle(UpdateClientCommand updateClientCommand, CancellationToken cancellationToken)
    {
        //TODO Back : High-performance logging with LoggerMessage #19
        logger.LogInformation("Attempting to update client with ID: {Id}", updateClientCommand.Id);

        var clientToUpdate = await _context.Client.FindAsync(updateClientCommand.Id);
            
        if (clientToUpdate is null)
        {
            logger.LogWarning("Client with ID: {Id} not found", updateClientCommand.Id);
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

        logger.LogInformation("Client with ID: {Id} updated successfully", updateClientCommand.Id);

        return Result.Ok();
    }
}
