namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.CreatePriceQuote;

public class CreatePriceQuoteEnpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost(
           "/quotations",
           async Task<Results<Created<CreateQuotationRequest>, ValidationProblem>> (
               IMediator mediator,
               CreateQuotationRequest request,
               CancellationToken cancellationToken) =>
           {
               var createPriceQuoteCommand = new CreatePriceQuoteCommand
               (
                   Num: request.Num,
                   IdClient: request.IdClient,
                   Date: request.Date,
                   TotHTva: request.TotTva,
                   TotTva: request.TotTva,
                   TotTtc: request.TotTtc
                   );

               var result = await mediator.Send(createPriceQuoteCommand, cancellationToken);

               if (result.IsFailed)
               {
                   return result.ToValidationProblem();
               }

               return TypedResults.Created($"/quotations/{result.Value}", request);
           })
           .WithTags(SwaggerTags.PriceQuotes);
    }
}
