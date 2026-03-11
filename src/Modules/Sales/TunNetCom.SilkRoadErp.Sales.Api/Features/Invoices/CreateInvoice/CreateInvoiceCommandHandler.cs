using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice
{
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<int>>
    {
        private readonly SalesContext _context;
        private readonly ILogger<CreateInvoiceCommandHandler> _logger;
        private readonly INumberGeneratorService _numberGeneratorService;
        public CreateInvoiceCommandHandler(SalesContext context, ILogger<CreateInvoiceCommandHandler> logger, INumberGeneratorService numberGeneratorService)
        {
            _context = context;
            _logger = logger;
            _numberGeneratorService = numberGeneratorService;
        }
        public async Task<Result<int>> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateInvoiceCommand called with ClientId {ClientId} and Date {Date}", command.ClientId, command.Date);
            var clientExists = await _context.Client.AnyAsync(c => c.Id == command.ClientId, cancellationToken);
            if (!clientExists)
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

            var num = await _numberGeneratorService.GenerateFactureNumberAsync(activeAccountingYear.Id, cancellationToken);

            var invoice = new Facture
            {
                Num = num,
                Date = command.Date,
                IdClient = command.ClientId,
                AccountingYearId = activeAccountingYear.Id
            };
            _ = _context.Facture.Add(invoice);
            _ = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Facture created successfully with Id {Id} and Num {Num}", invoice.Id, invoice.Num);
            return Result.Ok(invoice.Id);
        }
    }
}
