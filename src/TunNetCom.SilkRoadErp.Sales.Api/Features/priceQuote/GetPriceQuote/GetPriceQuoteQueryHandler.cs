using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomer;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuote
{
    public class GetPriceQuoteQueryHandler(
    SalesContext _context,
    ILogger<GetCustomerQueryHandler> _logger) 
    : IRequestHandler<GetPriceQuoteQuery, PagedList<QuotationResponse>>
    {
       
        public async Task<PagedList<QuotationResponse>> Handle(GetPriceQuoteQuery getPriceQuoteQuery, CancellationToken cancellationToken)
        {
            _logger.LogPaginationRequest(nameof(Devis), getPriceQuoteQuery.PageNumber, getPriceQuoteQuery.PageSize);

            var DevisQuery = _context.Devis.Select(t =>
                new QuotationResponse
                {
                    Num = t.Num,
                    IdClient = t.IdClient,
                    Date = t.Date,
                    TotHTva = t.TotHTva,
                    TotTva = t.TotTva,
                    TotTtc = t.TotTtc
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(getPriceQuoteQuery.SearchKeyword))
            {
                DevisQuery = DevisQuery.Where(
                    q => q.Num.ToString().Contains(getPriceQuoteQuery.SearchKeyword)
                    || q.IdClient.ToString().Contains(getPriceQuoteQuery.SearchKeyword)
                    || q.Date.ToString("yyyy-MM-dd").Contains(getPriceQuoteQuery.SearchKeyword)
                    || q.TotHTva.ToString().Contains(getPriceQuoteQuery.SearchKeyword)
                    || q.TotTva.ToString().Contains(getPriceQuoteQuery.SearchKeyword)
                    || q.TotTtc.ToString().Contains(getPriceQuoteQuery.SearchKeyword));
            }

            var pagedQuotations = await PagedList<QuotationResponse>.ToPagedListAsync(
                DevisQuery,
                getPriceQuoteQuery.PageNumber,
                getPriceQuoteQuery.PageSize,
                cancellationToken);

            _logger.LogEntitiesFetched(nameof(Devis), pagedQuotations.Count);

            return pagedQuotations;
        }
    }
}
