namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.UpdateClient;

public class UpdateClientCommandHandler(SalesContext _context ,IValidator<UpdateClientCommand> validator)
    : IRequestHandler<UpdateClientCommand>
{
    public async Task Handle(UpdateClientCommand updateClientCommand, CancellationToken cancellationToken)
    {
        var clientToUpdate = await _context.Client.FindAsync(updateClientCommand.Id);
        ValidationResult validationResult = await validator.ValidateAsync(updateClientCommand, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
            
        if (clientToUpdate is null)
        {
            
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
        return;
    }
}
