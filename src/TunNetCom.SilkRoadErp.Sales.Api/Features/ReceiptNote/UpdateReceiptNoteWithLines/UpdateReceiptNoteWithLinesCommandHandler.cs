using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.UpdateReceiptNoteWithLines;

public class UpdateReceiptNoteWithLinesCommandHandler(
    SalesContext _context,
    ILogger<UpdateReceiptNoteWithLinesCommandHandler> _logger,
    IActiveAccountingYearService _activeAccountingYearService,
    IAccountingYearFinancialParametersService _financialParametersService)
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

        // Get active accounting year ID
        var activeAccountingYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);
        if (!activeAccountingYearId.HasValue)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        // Calculate totals from new lines (before adding them)
        var totHTva = 0m;
        var totTva = 0m;
        var netPayer = 0m;

        // Get FODEC rate from financial parameters service
        var fodecRate = await _financialParametersService.GetPourcentageFodecAsync(0, cancellationToken);

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

            // Use centralized FODEC calculator for constructor suppliers
            var (fodecAmount, updatedTotTtc) = ReceiptNoteFodecCalculator.CalculateFodecAndTtc(
                line.TotHt,
                line.Tva,
                fodecRate,
                isConstructor);
            
            if (isConstructor && line.TotHt > 0)
            {
                line.TotTtc = updatedTotTtc;
            }

            return line;
        }).ToList();

        // Calculate totals from new lines
        // Note: For constructor suppliers, TotTtc = HT + FODEC + TVA
        // So TotTtc - TotHt = FODEC + TVA (which is stored in TotTva)
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
            accountingYearId: activeAccountingYearId.Value,
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

