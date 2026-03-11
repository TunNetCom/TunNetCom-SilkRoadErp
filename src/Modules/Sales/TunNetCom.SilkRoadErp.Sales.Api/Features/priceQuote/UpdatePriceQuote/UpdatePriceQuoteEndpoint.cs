namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.UpdatePriceQuote;

public class UpdatePriceQuoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/quotations/{num:int}",
        async Task<Results<NoContent, NotFound, ValidationProblem>> (
            IMediator mediator, int num,
            UpdateQuotationRequest request,
            CancellationToken cancellationToken) =>
        {
            var quotationLines = request.Items.Select(item => new CreatePriceQuote.LigneDevisSubCommand
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

            var updatePriceQuoteCommand = new UpdatePriceQuoteCommand(
                Num: num,
                IdClient: request.IdClient,
                Date: request.Date,
                TotHTva: request.TotHTva,
                TotTva: request.TotTva,
                TotTtc: request.TotTtc,
                QuotationLines: quotationLines);

            var updateQuotationResult = await mediator.Send(updatePriceQuoteCommand, cancellationToken);

            if (updateQuotationResult.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }

            if (updateQuotationResult.IsFailed)
            {
                return updateQuotationResult.ToValidationProblem();
            }

            return TypedResults.NoContent();
        })
        .WithTags(EndpointTags.PriceQuotes);
    }
}
