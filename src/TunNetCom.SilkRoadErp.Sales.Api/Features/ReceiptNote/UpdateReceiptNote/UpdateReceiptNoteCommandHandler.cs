namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.UpdateReceiptNote;

public class UpdateReceiptNoteCommandHandler(
    SalesContext _context,
    ILogger<UpdateReceiptNoteCommandHandler> _logger): IRequestHandler<UpdateReceiptNoteCommand, Result>
{
    public async Task<Result> Handle(UpdateReceiptNoteCommand updateReceiptNoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(BonDeReception), updateReceiptNoteCommand.Num);
        var receiptnote = await _context.BonDeReception.FindAsync(updateReceiptNoteCommand.Num);

        if (receiptnote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeReception), updateReceiptNoteCommand.Num);

            return Result.Fail(EntityNotFound.Error());
        }
        receiptnote.UpdateReceiptNote(
            num: updateReceiptNoteCommand.Num,
            numBonFournisseur: updateReceiptNoteCommand.NumBonFournisseur,
            dateLivraison: updateReceiptNoteCommand.DateLivraison,
            idFournisseur: updateReceiptNoteCommand.IdFournisseur,
            date: updateReceiptNoteCommand.Date,
            numFactureFournisseur: updateReceiptNoteCommand.NumFactureFournisseur
            );
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(BonDeReception), updateReceiptNoteCommand.Num);

        return Result.Ok();
    }
}
