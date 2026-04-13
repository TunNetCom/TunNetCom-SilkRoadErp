namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.DeletePriceQuote;

public class DeletePriceQuoteCommand : IRequest<Result>
{
    public int Num { get; }

    public DeletePriceQuoteCommand(int num)
    {
        Num = num;
    }
}
