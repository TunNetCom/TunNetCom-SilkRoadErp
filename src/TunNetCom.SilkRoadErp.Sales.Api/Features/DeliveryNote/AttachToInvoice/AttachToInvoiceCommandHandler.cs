using Azure.Core;
using MediatR;
using TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.AttachToInvoice;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public class AttachToInvoiceCommandHandler(
    SalesContext _context,
    ILogger<AttachToInvoiceCommandHandler> _logger)
    : IRequestHandler<AttachToInvoiceCommand, Result>
{
    public async Task<Result> Handle(AttachToInvoiceCommand attachToInvoiceCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(BonDeLivraison), attachToInvoiceCommand);

        // Fetch only the required fields from Facture
        var invoice = await _context.Facture
            .Select(i => new { CustomerId = i.IdClient, NumInvoice = i.Num })
            .FirstOrDefaultAsync(i => i.NumInvoice == attachToInvoiceCommand.InvoiceId, cancellationToken);

        if (invoice is null)
        {
            _logger.LogEntityNotFound(nameof(Facture), attachToInvoiceCommand.InvoiceId);
            return Result.Fail(EntityNotFound.Error);
        }

        // Fetch only the necessary fields from BonDeLivraison to validate
        var deliveryNote = await _context.BonDeLivraison
            .Where(d => d.Num == attachToInvoiceCommand.DeliveryNoteId)
            .Select(d => new { d.NumFacture, d.ClientId })
            .FirstOrDefaultAsync(cancellationToken);

        if (deliveryNote is null)
        {
            _logger.LogEntityNotFound(nameof(BonDeLivraison), attachToInvoiceCommand.DeliveryNoteId);
            return Result.Fail(EntityNotFound.Error);
        }

        if (deliveryNote.NumFacture.HasValue)
        {
            _logger.LogWarning("Delivery note with Num {DeliveryNoteId} is already attached to Facture {FactureId}.",
                attachToInvoiceCommand.DeliveryNoteId, deliveryNote.NumFacture);
            return Result.Fail("delivery_note_already_attached");
        }

        if (deliveryNote.ClientId != invoice.CustomerId)
        {
            _logger.LogWarning("Delivery note with Num {DeliveryNoteId} is not for the same client as Facture {FactureId}.",
                attachToInvoiceCommand.DeliveryNoteId, deliveryNote.NumFacture);
            return Result.Fail("customer_invoice_doesnt_match_customer_delivery_note");
        }

        // Perform a direct update without loading the full entity
        var rowsAffected = await _context.BonDeLivraison
            .Where(d => d.Num == attachToInvoiceCommand.DeliveryNoteId)
            .ExecuteUpdateAsync(
                updates => updates.SetProperty(d => d.NumFacture, attachToInvoiceCommand.InvoiceId),
                cancellationToken);

        if (rowsAffected == 0)
        {
            _logger.LogError("Failed to update delivery note {DeliveryNoteId}.", attachToInvoiceCommand.DeliveryNoteId);
            return Result.Fail("update_failed");
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Attached delivery note {DeliveryNoteId} to facture {FactureId}.",
            attachToInvoiceCommand.DeliveryNoteId, attachToInvoiceCommand.InvoiceId);

        return Result.Ok();
    }
}