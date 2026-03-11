namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.TransferInvoiceToCustomer;

public class TransferInvoiceToCustomerCommandHandler(
    SalesContext _context,
    ILogger<TransferInvoiceToCustomerCommandHandler> _logger)
    : IRequestHandler<TransferInvoiceToCustomerCommand, Result>
{
    public async Task<Result> Handle(TransferInvoiceToCustomerCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("TransferInvoiceToCustomerCommand called for invoice {InvoiceNumber} to customer {TargetCustomerId}",
            command.InvoiceNumber, command.TargetCustomerId);

        // Vérifier que la facture existe
        var invoice = await _context.Facture
            .Include(f => f.BonDeLivraison)
            .AsTracking()
            .FirstOrDefaultAsync(f => f.Num == command.InvoiceNumber, cancellationToken);

        if (invoice == null)
        {
            _logger.LogEntityNotFound(nameof(Facture), command.InvoiceNumber);
            return Result.Fail(EntityNotFound.Error());
        }

        // Vérifier que la facture n'a pas de BLs
        if (invoice.BonDeLivraison != null && invoice.BonDeLivraison.Any())
        {
            _logger.LogWarning("Invoice {InvoiceNumber} has {Count} delivery notes attached, cannot transfer",
                command.InvoiceNumber, invoice.BonDeLivraison.Count);
            return Result.Fail("invoice_has_delivery_notes");
        }

        // Vérifier que la facture est en statut Draft
        if (invoice.Statut != DocumentStatus.Draft)
        {
            _logger.LogWarning("Invoice {InvoiceNumber} is not in draft status (current: {Statut}), cannot transfer",
                command.InvoiceNumber, invoice.Statut);
            return Result.Fail("invoice_must_be_draft");
        }

        // Vérifier que le client cible existe
        var targetCustomerExists = await _context.Client
            .AnyAsync(c => c.Id == command.TargetCustomerId, cancellationToken);

        if (!targetCustomerExists)
        {
            _logger.LogEntityNotFound(nameof(Client), command.TargetCustomerId);
            return Result.Fail(EntityNotFound.Error());
        }

        // Vérifier que le client cible est différent du client actuel
        var oldCustomerId = invoice.IdClient;
        if (oldCustomerId == command.TargetCustomerId)
        {
            _logger.LogWarning("Target customer {TargetCustomerId} is the same as current customer for invoice {InvoiceNumber}",
                command.TargetCustomerId, command.InvoiceNumber);
            return Result.Fail("target_customer_same_as_current");
        }

        // Mettre à jour le client de la facture
        invoice.IdClient = command.TargetCustomerId;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully transferred invoice {InvoiceNumber} from customer {OldCustomerId} to customer {NewCustomerId}",
            command.InvoiceNumber, oldCustomerId, command.TargetCustomerId);

        return Result.Ok();
    }
}

