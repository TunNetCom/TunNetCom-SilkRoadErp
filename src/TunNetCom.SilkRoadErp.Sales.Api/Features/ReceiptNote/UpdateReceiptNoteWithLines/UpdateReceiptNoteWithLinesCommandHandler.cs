namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.UpdateReceiptNoteWithLines;

public class UpdateReceiptNoteWithLinesCommandHandler(
    SalesContext _context,
    ILogger<UpdateReceiptNoteWithLinesCommandHandler> _logger)
    : IRequestHandler<UpdateReceiptNoteWithLinesCommand, Result>
{
    public async Task<Result> Handle(
        UpdateReceiptNoteWithLinesCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(BonDeReception), command.Num);

        var receiptNote = await _context.BonDeReception
            .Include(b => b.LigneBonReception)
            .FirstOrDefaultAsync(b => b.Num == command.Num, cancellationToken);

        if (receiptNote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeReception), command.Num);
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

        // Update the receipt note properties
        receiptNote.UpdateReceiptNote(
            num: command.Num,
            numBonFournisseur: command.NumBonFournisseur,
            dateLivraison: command.DateLivraison,
            idFournisseur: command.IdFournisseur,
            date: command.Date,
            numFactureFournisseur: command.NumFactureFournisseur,
            accountingYearId: activeAccountingYear.Id
        );

        // Remove all existing lines
        _context.LigneBonReception.RemoveRange(receiptNote.LigneBonReception);

        // Create new lines
        var newLines = command.ReceiptNoteLines.Select(receiptNoteLine => 
            LigneBonReception.CreateReceiptNoteLine(
                receiptNoteNumber: receiptNote.Num,
                productRef: receiptNoteLine.ProductRef,
                designationLigne: receiptNoteLine.ProductDescription,
                quantity: receiptNoteLine.Quantity,
                unitPrice: receiptNoteLine.UnitPrice,
                discount: receiptNoteLine.Discount,
                tax: receiptNoteLine.Tax
            )).ToList();

        // Add new lines to the context explicitly
        await _context.LigneBonReception.AddRangeAsync(newLines, cancellationToken);

        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated receipt note {Num} with {LineCount} lines", command.Num, newLines.Count);

        _logger.LogEntityUpdated(nameof(BonDeReception), command.Num);

        return Result.Ok();
    }
}

