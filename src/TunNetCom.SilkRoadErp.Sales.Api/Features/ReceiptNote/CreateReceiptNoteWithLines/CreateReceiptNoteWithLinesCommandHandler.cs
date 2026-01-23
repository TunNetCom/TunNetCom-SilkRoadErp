using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteWithLines;

internal class CreateReceiptNoteWithLinesCommandHandler(
    SalesContext salesContext,
    ILogger<CreateReceiptNoteWithLinesCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService,
    IActiveAccountingYearService _activeAccountingYearService,
    IAccountingYearFinancialParametersService _financialParametersService) 
    : IRequestHandler<CreateReceiptNoteWithLigneCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateReceiptNoteWithLigneCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await salesContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Get the active accounting year ID
            var activeAccountingYearId = await _activeAccountingYearService.GetActiveAccountingYearIdAsync(cancellationToken);

            if (!activeAccountingYearId.HasValue)
            {
                _logger.LogError("No active accounting year found");
                return Result.Fail("no_active_accounting_year");
            }

            var num = await _numberGeneratorService.GenerateBonDeReceptionNumberAsync(activeAccountingYearId.Value, cancellationToken);

            var recipetNote = new BonDeReception
            {
                Num = num,
                NumBonFournisseur = request.NumBonFournisseur,
                DateLivraison = request.DateLivraison,
                IdFournisseur = request.IdFournisseur,
                Date = request.Date,
                NumFactureFournisseur = request.NumFactureFournisseur,
                AccountingYearId = activeAccountingYearId.Value
            };

            await salesContext.BonDeReception.AddAsync(recipetNote, cancellationToken);
            
            // Save the receipt note first to get its Id
            await salesContext.SaveChangesAsync(cancellationToken);

            // Get FODEC rate from active accounting year using the financial parameters service
            var fodecRate = await _financialParametersService.GetPourcentageFodecAsync(0, cancellationToken);

            // Get provider to check if it's a constructor
            var provider = await salesContext.Fournisseur
                .FirstOrDefaultAsync(f => f.Id == request.IdFournisseur, cancellationToken);
            var isConstructor = provider?.Constructeur ?? false;

            // Now create the lines with the saved receipt note's Id
            var receiptNoteLines = request.ReceiptNoteLines.Select(x =>
            {
                var line = LigneBonReception.CreateReceiptNoteLine(
                    bonDeReceptionId: recipetNote.Id,
                    productRef: x.ProductRef,
                    designationLigne: x.ProductDescription,
                    quantity: x.Quantity,
                    unitPrice: x.UnitPrice,
                    discount: x.Discount,
                    tax: x.Tax);

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

            await salesContext.LigneBonReception.AddRangeAsync(receiptNoteLines, cancellationToken);

            // Calculate totals from lines
            // Note: For constructor suppliers, TotTtc = HT + FODEC + TVA
            // So TotTtc - TotHt = FODEC + TVA (which is stored in TotTva)
            var totHTva = DecimalHelper.RoundAmount(receiptNoteLines.Sum(l => l.TotHt));
            var totTva = DecimalHelper.RoundAmount(receiptNoteLines.Sum(l => l.TotTtc - l.TotHt));
            var netPayer = DecimalHelper.RoundAmount(receiptNoteLines.Sum(l => l.TotTtc));

            // Update receipt note with calculated totals
            recipetNote.TotHTva = totHTva;
            recipetNote.TotTva = totTva;
            recipetNote.NetPayer = netPayer;

            // Save the lines and updated totals
            _ = await salesContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(recipetNote.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "error_creating_receiptNoteLines");
            return Result.Fail("error_creating_receiptNoteLines");
        }
    }
}
