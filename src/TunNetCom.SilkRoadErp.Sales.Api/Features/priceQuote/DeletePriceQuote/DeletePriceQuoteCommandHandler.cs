using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.DeleteCustomer;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.DeletePriceQuote;

public class DeletePriceQuoteCommandHandler (SalesContext _context,
ILogger<DeletePriceQuoteCommandHandler> _logger) : IRequestHandler<DeletePriceQuoteCommand, Result>
{
    public async Task<Result> Handle(DeletePriceQuoteCommand deletePriceQuoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityDeletionAttempt(
            nameof(Devis),
            deletePriceQuoteCommand.Num);

        var devis = await _context.Devis.FindAsync(deletePriceQuoteCommand.Num);

        if (devis is null)
        {
            _logger.LogEntityNotFound(nameof(Devis), deletePriceQuoteCommand.Num);

            return Result.Fail("devis_not_found");
        }

        _context.Devis.Remove(devis);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityDeleted(
            nameof(Devis),
            deletePriceQuoteCommand.Num);

        return Result.Ok();
    }

   
}
