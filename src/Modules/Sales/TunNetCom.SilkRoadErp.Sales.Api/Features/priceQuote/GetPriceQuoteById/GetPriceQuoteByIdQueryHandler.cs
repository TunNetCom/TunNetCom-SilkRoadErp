using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuoteById;

public class GetPriceQuoteByIdQueryHandler(
    SalesContext _context,
    ILogger<GetPriceQuoteByIdQueryHandler> _logger)
    : IRequestHandler<GetPriceQuoteByIdQuery, Result<FullQuotationResponse>>
{
    public async Task<Result<FullQuotationResponse>> Handle(GetPriceQuoteByIdQuery getPriceQuoteByIdQuery, CancellationToken cancellationToken)
    {
        _logger.LogFetchingEntityById(nameof(Devis), getPriceQuoteByIdQuery.Num);

        var quotationResponse = await _context.Devis
            .Select(d => new FullQuotationResponse
            {
                Num = d.Num,
                Date = d.Date,
                CustomerId = d.IdClient,
                TotalExcludingTax = d.TotHTva,
                TotalVat = d.TotTva,
                TotalAmount = d.TotTtc,
                Items = d.LigneDevis.Select(l => new QuotationDetailResponse
                {
                    Id = l.IdLi,
                    ProductReference = l.RefProduit,
                    Description = l.DesignationLi,
                    Quantity = l.QteLi,
                    UnitPriceExcludingTax = l.PrixHt,
                    DiscountPercentage = l.Remise,
                    VatPercentage = l.Tva,
                    TotalExcludingTax = l.TotHt,
                    TotalIncludingTax = l.TotTtc
                }).ToList()
            })
            .FirstOrDefaultAsync(d => d.Num == getPriceQuoteByIdQuery.Num, cancellationToken);

        if (quotationResponse is null)
        {
            _logger.LogEntityNotFound(nameof(Devis), getPriceQuoteByIdQuery.Num);

            return Result.Fail(EntityNotFound.Error());
        }

        _logger.LogEntityFetchedById(nameof(Devis), getPriceQuoteByIdQuery.Num);

        return quotationResponse;
    }
}
