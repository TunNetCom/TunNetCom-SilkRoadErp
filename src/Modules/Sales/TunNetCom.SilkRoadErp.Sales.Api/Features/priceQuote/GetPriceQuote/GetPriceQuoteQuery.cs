namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuote;

public record GetPriceQuoteQuery(
 int PageNumber,
int PageSize,
string? SearchKeyword) : IRequest<PagedList<QuotationResponse>>;
