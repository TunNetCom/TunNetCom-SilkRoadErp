using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.GetPriceQuoteById
{
    public class GetPriceQuoteByIdEnpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            _ = app.MapGet("/quotations/{num:int}", async Task<Results<Ok<FullQuotationResponse>, NotFound>> (
            IMediator mediator,
            int num,
            CancellationToken cancellationToken) =>
            {
                var query = new GetPriceQuoteByIdQuery(num);

                var result = await mediator.Send(query, cancellationToken);

                if (result.IsEntityNotFound())
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.Ok(result.Value);
            })
            .WithTags(EndpointTags.PriceQuotes);
        }
    }
}
