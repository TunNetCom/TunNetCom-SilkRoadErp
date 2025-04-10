namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DetachReceiptNotesFromInvoice;

public class DetachReceiptNotesFromInvoiceCommandHandler(
    SalesContext context,
    ILogger<DetachReceiptNotesFromInvoiceCommandHandler> logger)
    : IRequestHandler<DetachReceiptNotesFromInvoiceCommand, Result>
{
    public async Task<Result> Handle(DetachReceiptNotesFromInvoiceCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Detaching delivery notes {DeliveryNoteIds} from invoice {InvoiceId}",
            string.Join(", ", request.ReceiptNoteIds), request.InvoiceId);
        var isInvoiceExist = await context.FactureFournisseur
            .AnyAsync(f => f.Num == request.InvoiceId, cancellationToken);

        if (!isInvoiceExist)
        {
            logger.LogWarning("Invoice with id {InvoiceId} not found", request.InvoiceId);
            return Result.Fail(EntityNotFound.Error());
        }

        var updatedCount = await context.BonDeReception
            .Where(d => request.ReceiptNoteIds.Contains(d.Num) && d.NumFactureFournisseur == request.InvoiceId)
            .ExecuteUpdateAsync(
            setters => setters.SetProperty(d => d.NumFactureFournisseur, (int?)null),
            cancellationToken);

        logger.LogInformation(
            "Successfully detached {Count} delivery notes from invoice {InvoiceId}",
            updatedCount,
            request.InvoiceId);
        return Result.Ok();
    }
}
