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
            var invoice = await context.FactureFournisseur
                .Select(f => new { f.Num, f.Statut })
                .FirstOrDefaultAsync(f => f.Num == request.InvoiceId, cancellationToken);

            if (invoice is null)
            {
                logger.LogWarning("Invoice with ID {InvoiceId} not found", request.InvoiceId);
                return Result.Fail("EntityNotFound"); 
            }

            if (invoice.Statut == DocumentStatus.Valid)
            {
                logger.LogWarning("Attempt to detach receipt notes from validated invoice {InvoiceId}", request.InvoiceId);
                return Result.Fail("invoice_is_valid");
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
