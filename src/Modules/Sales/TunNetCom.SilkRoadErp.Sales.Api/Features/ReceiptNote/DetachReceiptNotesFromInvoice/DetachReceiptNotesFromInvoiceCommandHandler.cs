namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DetachReceiptNotesFromInvoice
{
    public class DetachReceiptNotesFromInvoiceCommandHandler(
         SalesContext context,
         ILogger<DetachReceiptNotesFromInvoiceCommandHandler> logger)
         : IRequestHandler<DetachReceiptNotesFromInvoiceCommand, Result>
    {
        public async Task<Result> Handle(DetachReceiptNotesFromInvoiceCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Detaching receipt notes {ReceiptNoteIds} from invoice {InvoiceId}",
                string.Join(", ", request.ReceiptNoteIds), request.InvoiceId);
            var invoiceExists = await context.FactureFournisseur
                .AnyAsync(f => f.Num == request.InvoiceId, cancellationToken);
            if (!invoiceExists)
            {
                logger.LogWarning("Invoice with ID {InvoiceId} not found", request.InvoiceId);
                return Result.Fail("EntityNotFound"); 
            }
            var notesToDetach = await context.BonDeReception
                .Where(note => request.ReceiptNoteIds.Contains(note.Num) &&
                               note.NumFactureFournisseur == request.InvoiceId)
                .ToListAsync(cancellationToken);
            foreach (var note in notesToDetach)
            {
                note.NumFactureFournisseur = null;
            }
            var updated = await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully detached {Count} receipt notes from invoice {InvoiceId}",
                updated, request.InvoiceId);
            return Result.Ok();
        }
    }
}
