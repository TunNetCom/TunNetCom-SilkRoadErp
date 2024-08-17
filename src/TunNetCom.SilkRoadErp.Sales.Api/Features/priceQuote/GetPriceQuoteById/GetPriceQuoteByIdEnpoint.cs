namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuoteById
{
    public class GetPriceQuoteByIdEnpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/quotations/{num:int}", async Task<Results<Ok<QuotationResponse>, NotFound>> (
            IMediator mediator,
            int num,
            CancellationToken cancellationToken) =>
            {
                var query = new GetPriceQuoteByIdQuery(num);

                var result = await mediator.Send(query, cancellationToken);

                if (result.IsFailed)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok(result.Value);
            });
        }
    }
}
