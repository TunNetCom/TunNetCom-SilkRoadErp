using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.UpdateInvoiceDate;

public class UpdateInvoiceDateCommandHandler : IRequestHandler<UpdateInvoiceDateCommand, Result>
{
    private readonly SalesContext _context;
    private readonly ILogger<UpdateInvoiceDateCommandHandler> _logger;

    public UpdateInvoiceDateCommandHandler(SalesContext context, ILogger<UpdateInvoiceDateCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateInvoiceDateCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to update invoice date for {Num} to {Date}", command.Num, command.Date);

        var invoice = await _context.Facture
            .FirstOrDefaultAsync(f => f.Num == command.Num, cancellationToken);

        if (invoice == null)
        {
            _logger.LogWarning("Invoice not found: {Num}", command.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        if (invoice.Statut != DocumentStatus.Draft)
        {
            _logger.LogWarning("Attempt to modify date of invoice {Num} while status is {Statut}", command.Num, invoice.Statut);
            return Result.Fail("invoice_date_only_draft");
        }

        // Update date
        invoice.Date = command.Date;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Invoice {Num} date updated to {Date}", command.Num, command.Date);
        return Result.Ok();
    }
}
