namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.CreatePriceQuote;

public record CreatePriceQuoteCommand(
     int Num,
     int IdClient,
     DateTime Date,
     decimal TotHTva,
     decimal TotTva,
     decimal TotTtc
) : IRequest<Result<int>>;


