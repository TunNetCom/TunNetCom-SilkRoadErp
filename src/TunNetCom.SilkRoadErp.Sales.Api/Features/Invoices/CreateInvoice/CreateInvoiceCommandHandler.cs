namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.CreateInvoice;

public class CreateInvoiceCommandHandler(
    SalesContext _context, 
    ILogger<CreateInvoiceCommandHandler> _logger) 
    : IRequestHandler<CreateInvoiceCommand,Result<int>>
{
    public async Task<Result<int>> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(Facture), command);

        var clientExists = await _context.Client.AnyAsync(c => c.Id == command.ClientId, cancellationToken);
        if (!clientExists)
        {
            return Result.Fail("client_not_found");
        }

        var invoice = new Facture
        {
            Date = command.Date,
            IdClient = command.ClientId
        };

        _context.Facture.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityCreatedSuccessfully(nameof(Facture), invoice.Num);

        return Result.Ok(invoice.Num);
    }
}
