namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeleteDeliveryNote;

public class DeleteDeliveryNoteCommandHandler(SalesContext _context,
    ILogger<DeleteDeliveryNoteCommandHandler> _logger) : IRequestHandler<DeleteDeliveryNoteCommand, Result>
{
    public async Task<Result> Handle(DeleteDeliveryNoteCommand deleteDeliveryNoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt(
            "DeliveryNote",
            deleteDeliveryNoteCommand.Num);

        var deliveryNote = await _context.BonDeLivraison.FindAsync(deleteDeliveryNoteCommand.Num);

        if (deliveryNote is null)
        {
            _logger.LogEntityNotFound("DeliveryNote", deleteDeliveryNoteCommand.Num);

            return Result.Fail("DeliveryNote_not_found");
        }

        _context.BonDeLivraison.Remove(deliveryNote);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted(
            "DeliveryNote",
            deleteDeliveryNoteCommand.Num);

        return Result.Ok();
    }
}
