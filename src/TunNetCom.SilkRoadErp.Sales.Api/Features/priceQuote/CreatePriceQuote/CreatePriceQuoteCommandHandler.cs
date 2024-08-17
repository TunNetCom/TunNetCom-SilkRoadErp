using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.CreateCustomer;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.CreatePriceQuote;

public class CreatePriceQuoteCommandHandler(
    SalesContext _context,
    ILogger<CreatePriceQuoteCommandHandler> _logger)
    : IRequestHandler<CreatePriceQuoteCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePriceQuoteCommand createPriceQuoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityCreated(nameof(Devis), createPriceQuoteCommand);

        var isQuotationNameExist = await _context.Devis.AnyAsync(
            prq => prq.Num == createPriceQuoteCommand.Num,
            cancellationToken);

        if (isQuotationNameExist)
        {
            return Result.Fail("quotations_num_exist");
        }

        var devis = Devis.CreateDevis
        (
        createPriceQuoteCommand.Num,
        createPriceQuoteCommand.IdClient,
        createPriceQuoteCommand.Date,
        createPriceQuoteCommand.TotHTva,
        createPriceQuoteCommand.TotTva,
        createPriceQuoteCommand.TotTtc
        );
        _context.Devis.Add(devis);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogEntityCreatedSuccessfully(nameof(Devis), devis.Num);

        return devis.Num;
    }
}
