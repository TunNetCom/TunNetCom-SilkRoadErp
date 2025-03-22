namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetUninvoicedDeliveryNote;

public class GetUninvoicedDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/deliveryNote/uninvoiced/{clientId:int}", async Task<Results<Ok<List<DeliveryNoteResponse>>, NotFound>> (
            IMediator mediator,
            int clientId,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUninvoicedDeliveryNoteQuery(clientId);

            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(result.Value);
        });
    }
}
