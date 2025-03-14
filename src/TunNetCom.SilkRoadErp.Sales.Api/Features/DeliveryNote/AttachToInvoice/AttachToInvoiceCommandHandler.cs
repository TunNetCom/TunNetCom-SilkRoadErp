using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.AttachToInvoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public class AttachToInvoiceCommandHandler(
    SalesContext _context,
    ILogger<AttachToInvoiceCommandHandler> _logger)
    : IRequestHandler<AttachToInvoiceCommand, Result>
{
    public async Task<Result> Handle(AttachToInvoiceCommand attachToInvoiceCommand, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to attach delivery notes {DeliveryNoteIds} to invoice {InvoiceId}",
            string.Join(", ", attachToInvoiceCommand.DeliveryNoteIds), attachToInvoiceCommand.InvoiceId);

        var invoice = await _context.Facture
            .Select(i => new { CustomerId = i.IdClient, NumInvoice = i.Num })
            .FirstOrDefaultAsync(i => i.NumInvoice == attachToInvoiceCommand.InvoiceId, cancellationToken);

        if (invoice is null)
        {
            _logger.LogEntityNotFound(nameof(Facture), attachToInvoiceCommand.InvoiceId);
            return Result.Fail(EntityNotFound.Error);
        }

        var deliveryNotes = await _context.BonDeLivraison
            .Where(d => attachToInvoiceCommand.DeliveryNoteIds.Contains(d.Num))
            .Select(d => new { d.Num, d.NumFacture, d.ClientId })
            .ToListAsync(cancellationToken);

        // Check if all requested delivery notes exist
        var missingDeliveryNotes = attachToInvoiceCommand.DeliveryNoteIds
            .Except(deliveryNotes.Select(d => d.Num))
            .ToList();
        if (missingDeliveryNotes.Any())
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), string.Join(", ", missingDeliveryNotes));
            return Result.Fail(EntityNotFound.Error);
        }

        // Check if any delivery notes are already attached to an invoice
        var alreadyAttached = deliveryNotes
            .Where(d => d.NumFacture.HasValue)
            .ToList();
        if (alreadyAttached.Any())
        {
            _logger.LogWarning("Delivery notes {DeliveryNoteIds} are already attached to invoices {FactureIds}",
                string.Join(", ", alreadyAttached.Select(d => d.Num)),
                string.Join(", ", alreadyAttached.Select(d => d.NumFacture)));
            return Result.Fail("delivery_note_already_attached");
        }

        // Check if all delivery notes belong to the same customer as the invoice
        var mismatchedCustomers = deliveryNotes
            .Where(d => d.ClientId != invoice.CustomerId)
            .ToList();
        if (mismatchedCustomers.Any())
        {
            _logger.LogWarning("Delivery notes {DeliveryNoteIds} do not match the customer of invoice {InvoiceId}",
                string.Join(", ", mismatchedCustomers.Select(d => d.Num)), attachToInvoiceCommand.InvoiceId);
            return Result.Fail("customer_invoice_doesnt_match_customer_delivery_note");
        }

        await _context.BonDeLivraison
            .Where(d => attachToInvoiceCommand.DeliveryNoteIds.Contains(d.Num))
            .ExecuteUpdateAsync(
                updates => updates.SetProperty(d => d.NumFacture, attachToInvoiceCommand.InvoiceId),
                cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully attached delivery notes {DeliveryNoteIds} to invoice {InvoiceId}",
            string.Join(", ", attachToInvoiceCommand.DeliveryNoteIds), attachToInvoiceCommand.InvoiceId);

        return Result.Ok();
    }
}