namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByClientId;

public class GetDeliveryNoteByClientIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/deliveryNote/client/{clientId:int}", async Task<Results<Ok<List<DeliveryNoteResponse>>, NotFound>> (
            IMediator mediator,
            int clientId,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDeliveryNoteByClientIdQuery(clientId);

            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(result.Value);
        });
    }
}
