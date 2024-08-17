
using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.DeleteCustomer;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.DeletePriceQuote;

public class DeletePriceQuoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/quotations/{num:int}", async Task<Results<NoContent, NotFound>> (
             IMediator mediator,
             int num,
             CancellationToken cancellationToken) =>
        {
            var deletePriceQuoteCommand = new DeletePriceQuoteCommand(num);
            var deleteResult = await mediator.Send(deletePriceQuoteCommand, cancellationToken);

            //TODO conditions based on the result : business validations and not found case.
            if (deleteResult.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        });
    }
}
