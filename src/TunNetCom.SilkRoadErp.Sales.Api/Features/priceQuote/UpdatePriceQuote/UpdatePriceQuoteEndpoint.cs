namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.UpdatePriceQuote
{
    public class UpdatePriceQuoteEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/quotations/{num:int}",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator, int num,
                UpdateQuotationRequest request,
                CancellationToken cancellationToken) =>
            {
                var updatePriceQuoteCommand = new UpdatePriceQuoteCommand(
                    Num: num,
                    IdClient: request.IdClient,
                    Date : request.Date,
                    TotHTva: request.TotHTva,
                    TotTva: request.TotTva,
                    TotTtc : request.TotTtc);

                var updateQuotationResult = await mediator.Send(updatePriceQuoteCommand, cancellationToken);

                if (updateQuotationResult.HasError<EntityNotFound>())
                {
                    return TypedResults.NotFound();
                }

                if (updateQuotationResult.IsFailed)
                {
                    return updateQuotationResult.ToValidationProblem();
                }

                return TypedResults.NoContent();
            });
        }
    }
}
