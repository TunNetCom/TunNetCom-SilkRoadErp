using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.UpdateProviderInvoice;

public class UpdateProviderInvoiceCommandHandler : IRequestHandler<UpdateProviderInvoiceCommand, Result>
{
    private readonly SalesContext _context;
    private readonly ILogger<UpdateProviderInvoiceCommandHandler> _logger;
    
    public UpdateProviderInvoiceCommandHandler(
        SalesContext context,
        ILogger<UpdateProviderInvoiceCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Result> Handle(UpdateProviderInvoiceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateProviderInvoiceCommand called with Num {Num} and NumFactureFournisseur {NumFactureFournisseur}", command.Num, command.NumFactureFournisseur);
        
        var invoice = await _context.FactureFournisseur
            .FirstOrDefaultAsync(f => f.Num == command.Num, cancellationToken);
        
        if (invoice == null)
        {
            _logger.LogEntityNotFound(nameof(FactureFournisseur), command.Num);
            return Result.Fail(EntityNotFound.Error());
        }
        
        invoice.NumFactureFournisseur = command.NumFactureFournisseur;
        
        _ = await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("FactureFournisseur updated successfully with Num {Num} and NumFactureFournisseur {NumFactureFournisseur}", invoice.Num, invoice.NumFactureFournisseur);
        return Result.Ok();
    }
}

