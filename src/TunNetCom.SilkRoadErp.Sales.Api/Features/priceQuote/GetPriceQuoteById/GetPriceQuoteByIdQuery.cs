using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuoteById;

public class GetPriceQuoteByIdQuery : IRequest<Result<FullQuotationResponse>>
{
    public int Num { get; set; }

    public GetPriceQuoteByIdQuery(int num)
    {
        Num = num;
    }
}
