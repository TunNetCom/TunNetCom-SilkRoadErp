namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuoteById;

public class GetPriceQuoteByIdQueryHandler(
    SalesContext _context,
    ILogger<GetPriceQuoteByIdQueryHandler> _logger)
    : IRequestHandler<GetPriceQuoteByIdQuery, Result<QuotationResponse>>
{
    public async Task<Result<QuotationResponse>> Handle(GetPriceQuoteByIdQuery getPriceQuoteByIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(Devis), getPriceQuoteByIdQuery.Num);

        var quotation = await _context.Devis.FindAsync(getPriceQuoteByIdQuery.Num, cancellationToken);

        if (quotation is null)
        {
            _logger.LogEntityNotFound(nameof(Devis), getPriceQuoteByIdQuery.Num);

            return Result.Fail("quotation_not_found");
        }

        _logger.LogEntityFetchedById(nameof(Devis), getPriceQuoteByIdQuery.Num);

        return quotation.Adapt<QuotationResponse>();
    }
}
