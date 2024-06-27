namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientCommandHandler(SalesContext _context ,IValidator<UpdateClientCommand> validator, 
    ILogger<UpdateClientCommandHandler> logger)
    : IRequestHandler<UpdateClientCommand>
{
    public async Task Handle(UpdateClientCommand updateClientCommand, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to update client with ID: {Id}", updateClientCommand.Id);

        var clientToUpdate = await _context.Client.FindAsync(updateClientCommand.Id);
        ValidationResult validationResult = await validator.ValidateAsync(updateClientCommand, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for client update: {@ValidationResult}", validationResult);
            throw new ValidationException(validationResult.Errors);
        }
            
        if (clientToUpdate is null)
        {
            logger.LogWarning("Client with ID: {Id} not found", updateClientCommand.Id);
            throw new KeyNotFoundException($"Client with Id {updateClientCommand.Id} not found.");
        }

        clientToUpdate.UpdateClient(
            nom: updateClientCommand.Nom,
            tel: updateClientCommand.Tel,
            adresse: updateClientCommand.Adresse,
            matricule: updateClientCommand.Matricule,
            code: updateClientCommand.Code,
            codeCat: updateClientCommand.CodeCat,
            etbSec: updateClientCommand.EtbSec,
            mail: updateClientCommand.Mail
            );

        await _context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Client with ID: {Id} updated successfully", updateClientCommand.Id);
        return;
    }
}
