using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.UpdateProviderInvoiceDate;

public class UpdateProviderInvoiceDateCommandHandler : IRequestHandler<UpdateProviderInvoiceDateCommand, Result>
{
    private readonly SalesContext _context;
    private readonly ILogger<UpdateProviderInvoiceDateCommandHandler> _logger;

    public UpdateProviderInvoiceDateCommandHandler(SalesContext context, ILogger<UpdateProviderInvoiceDateCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateProviderInvoiceDateCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to update provider invoice date for {Num} to {Date}", command.Num, command.Date);

        var invoice = await _context.FactureFournisseur
            .FirstOrDefaultAsync(f => f.Num == command.Num, cancellationToken);

        if (invoice == null)
        {
            _logger.LogWarning("Provider invoice not found: {Num}", command.Num);
            return Result.Fail(EntityNotFound.Error());
        }

        if (invoice.Statut != DocumentStatus.Draft)
        {
            _logger.LogWarning("Attempt to modify date of provider invoice {Num} while status is {Statut}", command.Num, invoice.Statut);
            return Result.Fail("invoice_date_only_draft");
        }

        invoice.Date = command.Date;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Provider invoice {Num} date updated to {Date}", command.Num, command.Date);
        return Result.Ok();
    }
}
