using TunNetCom.SilkRoadErp.Sales.Domain.Services;

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

        if (receiptNote.Statut == DocumentStatus.Valid)
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

        // Calculate totals from new lines (before adding them)
        var totHTva = 0m;
        var totTva = 0m;
        var netPayer = 0m;

        // Get system parameters for FODEC rate
        var systeme = await _context.Systeme.FirstOrDefaultAsync(cancellationToken);
        var fodecRate = systeme?.PourcentageFodec ?? 0;

        // Get provider to check if it's a constructor
        var provider = await _context.Fournisseur
            .FirstOrDefaultAsync(f => f.Id == command.IdFournisseur, cancellationToken);
        var isConstructor = provider?.Constructeur ?? false;

        // Remove all existing lines
        _context.LigneBonReception.RemoveRange(receiptNote.LigneBonReception);

        // Create new lines
        var newLines = command.ReceiptNoteLines.Select(receiptNoteLine =>
        {
            var line = LigneBonReception.CreateReceiptNoteLine(
                bonDeReceptionId: receiptNote.Id,
                productRef: receiptNoteLine.ProductRef,
                designationLigne: receiptNoteLine.ProductDescription,
                quantity: receiptNoteLine.Quantity,
                unitPrice: receiptNoteLine.UnitPrice,
                discount: receiptNoteLine.Discount,
                tax: receiptNoteLine.Tax
            );

            // Add FODEC to TotTtc if provider is constructor
            if (isConstructor && line.TotHt > 0)
            {
                var fodecAmount = DecimalHelper.RoundAmount(line.TotHt * (fodecRate / 100));
                line.TotTtc = DecimalHelper.RoundAmount(line.TotTtc + fodecAmount);
            }

            return line;
        }).ToList();

        // Calculate totals from new lines
        totHTva = DecimalHelper.RoundAmount(newLines.Sum(l => l.TotHt));
        totTva = DecimalHelper.RoundAmount(newLines.Sum(l => l.TotTtc - l.TotHt));
        netPayer = DecimalHelper.RoundAmount(newLines.Sum(l => l.TotTtc));

        // Update the receipt note properties with calculated totals
        receiptNote.UpdateReceiptNote(
            num: command.Num,
            numBonFournisseur: command.NumBonFournisseur,
            dateLivraison: command.DateLivraison,
            idFournisseur: command.IdFournisseur,
            date: command.Date,
            numFactureFournisseur: command.NumFactureFournisseur,
            accountingYearId: activeAccountingYear.Id,
            totHTva: totHTva,
            totTva: totTva,
            netPayer: netPayer
        );

        // Add new lines to the context explicitly
        await _context.LigneBonReception.AddRangeAsync(newLines, cancellationToken);

        _ = await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated receipt note {Num} with {LineCount} lines", command.Num, newLines.Count);

        _logger.LogEntityUpdated(nameof(BonDeReception), command.Num);

        return Result.Ok();
    }
}

