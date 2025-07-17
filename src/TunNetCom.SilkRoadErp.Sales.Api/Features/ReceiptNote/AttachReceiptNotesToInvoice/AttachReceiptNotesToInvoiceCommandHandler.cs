namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.AttachReceiptNotesToInvoice;
public class AttachReceiptNotesToInvoiceCommandHandler(
    SalesContext context,
    ILogger<AttachReceiptNotesToInvoiceCommandHandler> logger) : IRequestHandler<AttachReceiptNotesToInvoiceCommand, Result>
{
    public async Task<Result> Handle(AttachReceiptNotesToInvoiceCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attaching receipt notes {receipt-notes} to invoice {invoice}",
            string.Join(" ,", command.ReceiptNotesIds), command.InvoiceId);
        var invoiceExist = await context.ProviderInvoiceView
            .AnyAsync(f => f.Num == command.InvoiceId);

        if (!invoiceExist)
        {
            logger.LogWarning("Invoice with id {InvoiceId} not found", command.InvoiceId);
            return Result.Fail(EntityNotFound.Error());
        }
        var receiptNotes = context.BonDeReception
            .Where(r => command.ReceiptNotesIds.Contains(r.Num))
            .Select(r => new { r.Num, r.NumFactureFournisseur, r.IdFournisseur })
            .ToList();
        var missingReceiptNotes = command.ReceiptNotesIds
            .Except(receiptNotes.Select(r => r.Num))
             .ToList();
        if (missingReceiptNotes.Any())
        {
            logger.LogWarning("Receipt notes {missing-receipt-notes} not found", string.Join(" ,", missingReceiptNotes));
            return Result.Fail(EntityNotFound.Error());
        }
        var alreadyAttachedReceiptNotes = receiptNotes
            .Where(r => r.NumFactureFournisseur != null)
            .Select(r => r.Num)
            .ToList();
        if (alreadyAttachedReceiptNotes.Any())
        {
            logger.LogWarning("Receipt notes {already-attached-receipt-notes} are already attached to an invoice",
                string.Join(" ,", alreadyAttachedReceiptNotes));
            return Result.Fail("receipt_notes_already_attached");
        }
        await context.BonDeReception
            .Where(r => command.ReceiptNotesIds.Contains(r.Num))
            .ExecuteUpdateAsync(
                updates => updates.SetProperty(r => r.NumFactureFournisseur, command.InvoiceId),
                cancellationToken);
        logger.LogInformation("Successfully attached receipt notes {ReceiptNoteIds} to invoice {InvoiceId}",
            string.Join(", ", command.ReceiptNotesIds), command.InvoiceId);
        return Result.Ok();
    }
}
