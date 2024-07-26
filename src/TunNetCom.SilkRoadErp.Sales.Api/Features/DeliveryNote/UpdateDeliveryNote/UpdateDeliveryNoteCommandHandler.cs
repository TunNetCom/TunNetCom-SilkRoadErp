namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.UpdateDeliveryNote;

public class UpdateDeliveryNoteCommandHandler(
    SalesContext _context,
    ILogger<UpdateDeliveryNoteCommandHandler> _logger)
    : IRequestHandler<UpdateDeliveryNoteCommand, Result>
{
    public async Task<Result> Handle(UpdateDeliveryNoteCommand updateDeliveryNoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);

        var deliveryNoteToUpdate = await _context.BonDeLivraison.FindAsync(updateDeliveryNoteCommand.Num);

        if (deliveryNoteToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);

            return Result.Fail(EntityNotFound.Error);
        }

        deliveryNoteToUpdate.UpdateBonDeLivraison(
            date: updateDeliveryNoteCommand.Date,
            totHTva: updateDeliveryNoteCommand.TotHTva,
            totTva: updateDeliveryNoteCommand.TotTva,
            netPayer: updateDeliveryNoteCommand.NetPayer,
            tempBl: updateDeliveryNoteCommand.TempBl,
            numFacture: updateDeliveryNoteCommand.NumFacture,
            clientId: updateDeliveryNoteCommand.ClientId);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);

        return Result.Ok();
    }
}
