using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteWithLines;

internal class CreateReceiptNoteWithLinesCommandHandler(
    SalesContext salesContext,
    ILogger<CreateReceiptNoteWithLinesCommandHandler> _logger,
    INumberGeneratorService _numberGeneratorService) 
    : IRequestHandler<CreateReceiptNoteWithLigneCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateReceiptNoteWithLigneCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await salesContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {

            // Get the active accounting year
            var activeAccountingYear = await salesContext.AccountingYear
                .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

            if (activeAccountingYear == null)
            {
                _logger.LogError("No active accounting year found");
                return Result.Fail("no_active_accounting_year");
            }

            var num = await _numberGeneratorService.GenerateBonDeReceptionNumberAsync(activeAccountingYear.Id, cancellationToken);

            var recipetNote = new BonDeReception
            {
                Num = num,
                NumBonFournisseur = request.NumBonFournisseur,
                DateLivraison = request.DateLivraison,
                IdFournisseur = request.IdFournisseur,
                Date = request.Date,
                NumFactureFournisseur = request.NumFactureFournisseur,
                AccountingYearId = activeAccountingYear.Id
            };

            await salesContext.BonDeReception.AddAsync(recipetNote, cancellationToken);
            
            // Save the receipt note first to get its Id
            await salesContext.SaveChangesAsync(cancellationToken);

            // Get system parameters for FODEC rate
            var systeme = await salesContext.Systeme.FirstOrDefaultAsync(cancellationToken);
            var fodecRate = systeme?.PourcentageFodec ?? 0;

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

                // Add FODEC to TotTtc if provider is constructor
                if (isConstructor && line.TotHt > 0)
                {
                    var fodecAmount = DecimalHelper.RoundAmount(line.TotHt * (fodecRate / 100));
                    line.TotTtc = DecimalHelper.RoundAmount(line.TotTtc + fodecAmount);
                }

                return line;
            }).ToList();

            await salesContext.LigneBonReception.AddRangeAsync(receiptNoteLines, cancellationToken);

            // Calculate totals from lines
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
