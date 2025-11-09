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
            var receiptNoteLines = request.ReceiptNoteLines.Select(x => LigneBonReception.CreateReceiptNoteLine(
                                                receiptNoteNumber: x.RecipetNoteNumber,
                                                productRef: x.ProductRef,
                                                designationLigne: x.ProductDescription,
                                                quantity: x.Quantity,
                                                unitPrice: x.UnitPrice,
                                                discount: x.Discount,
                                                tax: x.Tax)).ToList();

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
