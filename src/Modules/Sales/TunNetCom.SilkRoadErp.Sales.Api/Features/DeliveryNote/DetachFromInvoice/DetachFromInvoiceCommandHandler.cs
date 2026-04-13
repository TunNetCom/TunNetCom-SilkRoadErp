using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DetachFromInvoice;

public class DetachFromInvoiceCommandHandler(
    SalesContext context,
    ILogger<DetachFromInvoiceCommandHandler> logger)
    : IRequestHandler<DetachFromInvoiceCommand, Result>
{
    public async Task<Result> Handle(DetachFromInvoiceCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Detaching delivery notes {DeliveryNoteIds} from invoice {InvoiceId}",
            string.Join(", ", request.DeliveryNoteIds), request.InvoiceId);

        var isInvoiceExist = await context.Facture
            .AnyAsync(f => f.Num == request.InvoiceId, cancellationToken);

        if (!isInvoiceExist)
        {
            logger.LogWarning("Invoice with id {InvoiceId} not found", request.InvoiceId);
            return Result.Fail("invoice_not_found");
        }

        var deliveryNotesToUpdate = await context.BonDeLivraison
            .Where(d => request.DeliveryNoteIds.Contains(d.Num) && d.NumFacture == request.InvoiceId)
            .ToListAsync(cancellationToken);

        foreach (var note in deliveryNotesToUpdate)
        {
            note.NumFacture = null;
        }

        _ = await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Successfully detached {Count} delivery notes from invoice {InvoiceId}",
            deliveryNotesToUpdate.Count,
            request.InvoiceId);

        return Result.Ok();
    }
}
