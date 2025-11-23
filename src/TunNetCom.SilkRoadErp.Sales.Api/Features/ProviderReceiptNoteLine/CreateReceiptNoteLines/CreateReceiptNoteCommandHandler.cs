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
            // Group lines by receipt note number to fetch all BonDeReception at once
            var receiptNoteNumbers = request.ReceiptNoteLines.Select(x => x.RecipetNoteNumber).Distinct().ToList();
            var receiptNotes = await salesContext.BonDeReception
                .Where(br => receiptNoteNumbers.Contains(br.Num))
                .ToDictionaryAsync(br => br.Num, br => br.Id, cancellationToken);

            var receiptNoteLines = new List<LigneBonReception>();
            foreach (var lineRequest in request.ReceiptNoteLines)
            {
                if (!receiptNotes.TryGetValue(lineRequest.RecipetNoteNumber, out var bonDeReceptionId))
                {
                    _logger.LogError("Receipt note with number {Num} not found", lineRequest.RecipetNoteNumber);
                    return Result.Fail($"Receipt note with number {lineRequest.RecipetNoteNumber} not found");
                }

                receiptNoteLines.Add(LigneBonReception.CreateReceiptNoteLine(
                    bonDeReceptionId: bonDeReceptionId,
                    productRef: lineRequest.ProductRef,
                    designationLigne: lineRequest.ProductDescription,
                    quantity: lineRequest.Quantity,
                    unitPrice: lineRequest.UnitPrice,
                    discount: lineRequest.Discount,
                    tax: lineRequest.Tax));
            }

            await salesContext.LigneBonReception.AddRangeAsync(receiptNoteLines, cancellationToken);
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
