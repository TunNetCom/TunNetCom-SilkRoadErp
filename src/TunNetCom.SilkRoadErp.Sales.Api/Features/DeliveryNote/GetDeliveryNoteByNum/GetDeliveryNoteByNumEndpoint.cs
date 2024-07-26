using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.GetCustomerById;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByNum;

public class GetDeliveryNoteByNumEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/deliveryNote/{num:int}", async Task<Results<Ok<DeliveryNoteResponse>, NotFound>> (
            IMediator mediator,
            int num,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDeliveryNoteByNumQuery(num);

            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(result.Value);
        });
    }
}
