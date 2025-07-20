namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice
{
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<int>>
    {
        private readonly SalesContext _context;
        private readonly ILogger<CreateInvoiceCommandHandler> _logger;
        public CreateInvoiceCommandHandler(SalesContext context, ILogger<CreateInvoiceCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Result<int>> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateInvoiceCommand called with ClientId {ClientId} and Date {Date}", command.ClientId, command.Date);
            var clientExists = await _context.Client.AnyAsync(c => c.Id == command.ClientId, cancellationToken);
            if (!clientExists)
            {
                return Result.Fail("not_found");
            }
            var invoice = new Facture
            {
                Date = command.Date,
                IdClient = command.ClientId
            };
            _context.Facture.Add(invoice);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Facture created successfully with Num {Num}", invoice.Num);
            return Result.Ok(invoice.Num);
        }
    }
}
