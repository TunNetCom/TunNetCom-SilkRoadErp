using Microsoft.Extensions.Logging;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DetachFromInvoice;

public class DetachFromInvoiceCommandHandler(
    SalesContext context,
    ILogger<DetachFromInvoiceCommandHandler> logger)
    : IRequestHandler<DetachFromInvoiceCommand, Result>
{
    public async Task<Result> Handle(DetachFromInvoiceCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Detaching delivery notes {DeliveryNoteIds} from invoice {InvoiceId}",
            string.Join(", ", request.DeliveryNoteIds), request.InvoiceId);

        // Verify the invoice exists
        var invoice = await context.Facture
            .FirstOrDefaultAsync(f => f.Num == request.InvoiceId, cancellationToken);

        if (invoice == null)
        {
            logger.LogWarning("Invoice {InvoiceId} not found", request.InvoiceId);
            return Result.Fail(new EntityNotFound());
        }

        // Fetch the delivery notes to detach
        var deliveryNotes = await context.BonDeLivraison
            .Where(d => request.DeliveryNoteIds.Contains(d.Num) && d.NumFacture == request.InvoiceId)
            .ToListAsync(cancellationToken);

        if (deliveryNotes.Count != request.DeliveryNoteIds.Count)
        {
            var notFoundIds = request.DeliveryNoteIds.Except(deliveryNotes.Select(d => d.Num)).ToList();
            logger.LogWarning("Some delivery notes not found or not associated with invoice {InvoiceId}: {NotFoundIds}",
                request.InvoiceId, string.Join(", ", notFoundIds));
            return Result.Fail(new EntityNotFound());
        }

        // Detach the delivery notes by setting NumFacture to null
        foreach (var deliveryNote in deliveryNotes)
        {
            deliveryNote.NumFacture = null;
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully detached {Count} delivery notes from invoice {InvoiceId}",
            deliveryNotes.Count, request.InvoiceId);

        return Result.Ok();
    }
}