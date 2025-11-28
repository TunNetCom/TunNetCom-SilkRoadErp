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
               var quotationLines = request.Items.Select(item => new LigneDevisSubCommand
               {
                   RefProduit = item.ProductReference,
                   DesignationLi = item.Description,
                   QteLi = item.Quantity,
                   PrixHt = item.UnitPriceExcludingTax,
                   Remise = item.DiscountPercentage,
                   TotHt = item.TotalExcludingTax,
                   Tva = item.VatPercentage,
                   TotTtc = item.TotalIncludingTax
               });

               var createPriceQuoteCommand = new CreatePriceQuoteCommand
               (
                   IdClient: request.IdClient,
                   Date: request.Date,
                   TotHTva: request.TotHTva,
                   TotTva: request.TotTva,
                   TotTtc: request.TotTtc,
                   QuotationLines: quotationLines
                   );

               var result = await mediator.Send(createPriceQuoteCommand, cancellationToken);

               if (result.IsFailed)
               {
                   return result.ToValidationProblem();
               }

               return TypedResults.Created($"/quotations/{result.Value}", request);
           })
           .RequireAuthorization("Permission:CanCreatePriceQuote")
           .WithTags(EndpointTags.PriceQuotes);
    }
}
