namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.UpdatePriceQuote;

public class UpdatePriceQuoteCommandHandler(
    SalesContext _context,
    ILogger<UpdatePriceQuoteCommandHandler> _logger)
    : IRequestHandler<UpdatePriceQuoteCommand, Result>
{
    public async Task<Result> Handle(UpdatePriceQuoteCommand updatePriceQuoteCommand, CancellationToken cancellationToken)
    {
        _logger.LogEntityUpdateAttempt(nameof(Devis), updatePriceQuoteCommand.Num);

        var quotationToUpdate = await _context.Devis.FindAsync(updatePriceQuoteCommand.Num);

        if (quotationToUpdate is null)
        {
            _logger.LogEntityNotFound(nameof(Devis), updatePriceQuoteCommand.Num);

            return Result.Fail(EntityNotFound.Error);
        }

        var isQuotationNumExist = await _context.Devis.AnyAsync(
            quo => quo.Num == updatePriceQuoteCommand.Num,
            cancellationToken);

        if (isQuotationNumExist)
        {
            return Result.Fail("quotation_num_exist");
        }

        quotationToUpdate.UpdateDevis(
            num: updatePriceQuoteCommand.Num,
            idClient: updatePriceQuoteCommand.IdClient,
            date: updatePriceQuoteCommand.Date,
            totHTva: updatePriceQuoteCommand.TotHTva,
            totTva: updatePriceQuoteCommand.TotTva,
            totTtc: updatePriceQuoteCommand.TotTtc);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogEntityUpdated(nameof(Devis), updatePriceQuoteCommand.Num);

        return Result.Ok();
    }
}
