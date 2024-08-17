namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.UpdatePriceQuote
{
    public record UpdatePriceQuoteCommand(
        int Num,
        int IdClient,
        DateTime Date,
        decimal TotHTva,
        decimal TotTva,
        decimal TotTtc
     ) : IRequest<Result>;
}
