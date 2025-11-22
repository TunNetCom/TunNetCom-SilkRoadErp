namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.UpdateDeliveryNote;

public class UpdateDeliveryNoteCommandHandler(
    SalesContext _context,
    ILogger<UpdateDeliveryNoteCommandHandler> _logger)
    : IRequestHandler<UpdateDeliveryNoteCommand, Result>
{
    public async Task<Result> Handle(
        UpdateDeliveryNoteCommand updateDeliveryNoteCommand,
        CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);

        var deliveryNote = await _context.BonDeLivraison
            .Include(b => b.LigneBl)
            .FirstOrDefaultAsync(b => b.Num == updateDeliveryNoteCommand.Num, cancellationToken);

        if (deliveryNote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        // Update the delivery note properties
        deliveryNote.UpdateBonDeLivraison(
            updateDeliveryNoteCommand.Date,
            updateDeliveryNoteCommand.TotHTva,
            updateDeliveryNoteCommand.TotTva,
            updateDeliveryNoteCommand.NetPayer,
            updateDeliveryNoteCommand.TempBl,
            updateDeliveryNoteCommand.NumFacture,
            updateDeliveryNoteCommand.ClientId
        );

        // Remove all existing lines
        _context.LigneBl.RemoveRange(deliveryNote.LigneBl);

        // Add new lines
        foreach (var deliveryNoteDetail in updateDeliveryNoteCommand.DeliveryNoteDetails)
        {
            var ligneBl = new LigneBl
            {
                RefProduit = deliveryNoteDetail.RefProduit,
                DesignationLi = deliveryNoteDetail.DesignationLi,
                QteLi = deliveryNoteDetail.QteLi,
                PrixHt = deliveryNoteDetail.PrixHt,
                Remise = deliveryNoteDetail.Remise,
                TotHt = deliveryNoteDetail.TotHt,
                Tva = deliveryNoteDetail.Tva,
                TotTtc = deliveryNoteDetail.TotTtc,
                NumBlNavigation = deliveryNote
            };

            deliveryNote.LigneBl.Add(ligneBl);
        }

        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);

        return Result.Ok();
    }
}

