namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeleteDeliveryNote;

public class DeleteDeliveryNoteCommandHandler(SalesContext _context,
    ILogger<DeleteDeliveryNoteCommandHandler> _logger) : IRequestHandler<DeleteDeliveryNoteCommand, Result>
{
    public async Task<Result> Handle(DeleteDeliveryNoteCommand deleteDeliveryNoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt(
            nameof(BonDeLivraison),
            deleteDeliveryNoteCommand.Num);

        var deliveryNote = await _context.BonDeLivraison.FindAsync(deleteDeliveryNoteCommand.Num);

        if (deliveryNote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), deleteDeliveryNoteCommand.Num);

            return Result.Fail(EntityNotFound.Error());
        }

        _ = _context.BonDeLivraison.Remove(deliveryNote);
        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted(
            nameof(BonDeLivraison),
            deleteDeliveryNoteCommand.Num);

        return Result.Ok();
    }
}
