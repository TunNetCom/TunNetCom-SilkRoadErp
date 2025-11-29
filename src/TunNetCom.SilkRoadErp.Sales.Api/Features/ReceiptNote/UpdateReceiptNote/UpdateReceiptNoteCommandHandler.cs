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

        if (receiptnote.Statut == DocumentStatus.Valide)
        {
            return Result.Fail("Le document est validé et ne peut plus être modifié.");
        }

        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        receiptnote.UpdateReceiptNote(
            num: updateReceiptNoteCommand.Num,
            numBonFournisseur: updateReceiptNoteCommand.NumBonFournisseur,
            dateLivraison: updateReceiptNoteCommand.DateLivraison,
            idFournisseur: updateReceiptNoteCommand.IdFournisseur,
            date: updateReceiptNoteCommand.Date,
            numFactureFournisseur: updateReceiptNoteCommand.NumFactureFournisseur,
            accountingYearId: activeAccountingYear.Id,
            totHTva: receiptnote.TotHTva,
            totTva: receiptnote.TotTva,
            netPayer: receiptnote.NetPayer
            );
        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(BonDeReception), updateReceiptNoteCommand.Num);

        return Result.Ok();
    }
}
