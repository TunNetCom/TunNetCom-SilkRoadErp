using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.CreateProviderInvoice;

public class CreateProviderInvoiceCommandHandler : IRequestHandler<CreateProviderInvoiceCommand, Result<int>>
{
    private readonly SalesContext _context;
    private readonly ILogger<CreateProviderInvoiceCommandHandler> _logger;
    private readonly INumberGeneratorService _numberGeneratorService;
    
    public CreateProviderInvoiceCommandHandler(
        SalesContext context, 
        ILogger<CreateProviderInvoiceCommandHandler> logger, 
        INumberGeneratorService numberGeneratorService)
    {
        _context = context;
        _logger = logger;
        _numberGeneratorService = numberGeneratorService;
    }
    
    public async Task<Result<int>> Handle(CreateProviderInvoiceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateProviderInvoiceCommand called with ProviderId {ProviderId} and Date {Date}", command.ProviderId, command.Date);
        
        var providerExists = await _context.Fournisseur.AnyAsync(f => f.Id == command.ProviderId, cancellationToken);
        if (!providerExists)
        {
            return Result.Fail("not_found");
        }
        
        // Get the active accounting year
        var activeAccountingYear = await _context.AccountingYear
            .FirstOrDefaultAsync(ay => ay.IsActive, cancellationToken);

        if (activeAccountingYear == null)
        {
            _logger.LogError("No active accounting year found");
            return Result.Fail("no_active_accounting_year");
        }

        var num = await _numberGeneratorService.GenerateFactureFournisseurNumberAsync(activeAccountingYear.Id, cancellationToken);

        var invoice = new FactureFournisseur
        {
            Num = num,
            Date = command.Date,
            DateFacturationFournisseur = command.Date,
            NumFactureFournisseur = num,
            IdFournisseur = command.ProviderId,
            Paye = false,
            AccountingYearId = activeAccountingYear.Id
        };
        
        _ = _context.FactureFournisseur.Add(invoice);
        _ = await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("FactureFournisseur created successfully with Id {Id} and Num {Num}", invoice.Id, invoice.Num);
        return Result.Ok(invoice.Num);
    }
}

