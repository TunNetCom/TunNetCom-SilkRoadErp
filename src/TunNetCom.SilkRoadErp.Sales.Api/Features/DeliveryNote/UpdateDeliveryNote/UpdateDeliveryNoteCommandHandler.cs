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

        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Update the delivery note properties
        deliveryNote.UpdateBonDeLivraison(
            updateDeliveryNoteCommand.Date,
            updateDeliveryNoteCommand.TotHTva,
            updateDeliveryNoteCommand.TotTva,
            updateDeliveryNoteCommand.NetPayer,
            updateDeliveryNoteCommand.TempBl,
            updateDeliveryNoteCommand.NumFacture,
            updateDeliveryNoteCommand.ClientId,
            activeAccountingYear.Id
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
                QteLivree = deliveryNoteDetail.QteLivree,
                PrixHt = deliveryNoteDetail.PrixHt,
                Remise = deliveryNoteDetail.Remise,
                TotHt = deliveryNoteDetail.TotHt,
                Tva = deliveryNoteDetail.Tva,
                TotTtc = deliveryNoteDetail.TotTtc,
                BonDeLivraisonId = deliveryNote.Id,
                NumBlNavigation = deliveryNote
            };

            deliveryNote.LigneBl.Add(ligneBl);
        }

        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(BonDeLivraison), updateDeliveryNoteCommand.Num);

        return Result.Ok();
    }
}

