namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuoteById;

public class GetPriceQuoteByIdQuery : IRequest<Result<QuotationResponse>>
{
    public int Num { get; set; }

    public GetPriceQuoteByIdQuery(int num)
    {
        Num = num;
    }
}
