namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteWithLines;

internal class CreateReceiptNoteWithLinesCommandHandler(
    SalesContext salesContext,
    ILogger<CreateReceiptNoteWithLinesCommandHandler> _logger) 
    : IRequestHandler<CreateReceiptNoteWithLigneCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateReceiptNoteWithLigneCommand request,
        CancellationToken cancellationToken)
    {
        await using var transaction = await salesContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {

            var recipetNote = await salesContext.BonDeReception.AddAsync(new BonDeReception
            {
                NumBonFournisseur = request.NumBonFournisseur,
                DateLivraison = request.DateLivraison,
                IdFournisseur = request.IdFournisseur,
                Date = request.Date,
                NumFactureFournisseur = request.NumFactureFournisseur,
            }, cancellationToken);

            var receiptNoteLines = request.ReceiptNoteLines.Select(x => LigneBonReception.CreateReceiptNoteLine(
                        receiptNoteNumber: recipetNote.Entity.Num,
                        productRef: x.ProductRef,
                        designationLigne: x.ProductDescription,
                        quantity: x.Quantity,
                        unitPrice: x.UnitPrice,
                        discount: x.Discount,
                        tax: x.Tax)).ToList();

             await salesContext.LigneBonReception.AddRangeAsync(receiptNoteLines, cancellationToken);

            _ = await salesContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Ok(recipetNote.Entity.Num);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "error_creating_receiptNoteLines");
            return Result.Fail("error_creating_receiptNoteLines");
        }
    }
}
