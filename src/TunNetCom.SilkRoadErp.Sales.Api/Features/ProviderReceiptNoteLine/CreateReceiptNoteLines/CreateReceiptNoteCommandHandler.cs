using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteLines;

internal class CreateReceiptNoteCommandHandler(
    SalesContext salesContext,
    ILogger<CreateReceiptNoteCommandHandler> _logger) 
    : IRequestHandler<CreateReceiptNoteLigneCommand, Result<List<int>>>
{
    public async Task<Result<List<int>>> Handle(
        CreateReceiptNoteLigneCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get system parameters for FODEC rate
            var systeme = await salesContext.Systeme.FirstOrDefaultAsync(cancellationToken);
            var fodecRate = systeme?.PourcentageFodec ?? 0;

            // Group lines by receipt note number to fetch all BonDeReception at once
            var receiptNoteNumbers = request.ReceiptNoteLines.Select(x => x.RecipetNoteNumber).Distinct().ToList();
            var receiptNotes = await salesContext.BonDeReception
                .Include(br => br.IdFournisseurNavigation)
                .Where(br => receiptNoteNumbers.Contains(br.Num))
                .ToDictionaryAsync(br => br.Num, br => new { br.Id, br.IdFournisseurNavigation }, cancellationToken);

            var receiptNoteLines = new List<LigneBonReception>();
            foreach (var lineRequest in request.ReceiptNoteLines)
            {
                if (!receiptNotes.TryGetValue(lineRequest.RecipetNoteNumber, out var receiptNoteInfo))
                {
                    _logger.LogError("Receipt note with number {Num} not found", lineRequest.RecipetNoteNumber);
                    return Result.Fail($"Receipt note with number {lineRequest.RecipetNoteNumber} not found");
                }

                var line = LigneBonReception.CreateReceiptNoteLine(
                    bonDeReceptionId: receiptNoteInfo.Id,
                    productRef: lineRequest.ProductRef,
                    designationLigne: lineRequest.ProductDescription,
                    quantity: lineRequest.Quantity,
                    unitPrice: lineRequest.UnitPrice,
                    discount: lineRequest.Discount,
                    tax: lineRequest.Tax);

                // Add FODEC to TotTtc if provider is constructor
                var isConstructor = receiptNoteInfo.IdFournisseurNavigation?.Constructeur ?? false;
                if (isConstructor && line.TotHt > 0)
                {
                    var fodecAmount = line.TotHt * (fodecRate / 100);
                    line.TotTtc += fodecAmount;
                }

                receiptNoteLines.Add(line);
            }

            await salesContext.LigneBonReception.AddRangeAsync(receiptNoteLines, cancellationToken);
            
            // Update totals for all affected receipt notes
            var receiptNoteIds = receiptNoteLines.Select(x => x.BonDeReceptionId).Distinct().ToList();
            var receiptNotesToUpdate = await salesContext.BonDeReception
                .Where(br => receiptNoteIds.Contains(br.Id))
                .Include(br => br.LigneBonReception)
                .ToListAsync(cancellationToken);

            foreach (var receiptNote in receiptNotesToUpdate)
            {
                var allLines = receiptNote.LigneBonReception.ToList();
                receiptNote.TotHTva = allLines.Sum(l => l.TotHt);
                receiptNote.TotTva = allLines.Sum(l => l.TotTtc - l.TotHt);
                receiptNote.NetPayer = allLines.Sum(l => l.TotTtc);
            }

            _ = await salesContext.SaveChangesAsync(cancellationToken);

            return Result.Ok(receiptNoteLines.Select(x => x.IdLigne).ToList());

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error_creating_receiptNoteLines");
            return Result.Fail("error_creating_receiptNoteLines");
        }
    }
}
