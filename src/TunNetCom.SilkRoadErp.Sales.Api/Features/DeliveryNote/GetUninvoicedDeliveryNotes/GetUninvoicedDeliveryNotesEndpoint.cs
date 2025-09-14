using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetUninvoicedDeliveryNotes;

public class GetUninvoicedDeliveryNotesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/deliveryNote/uninvoiced/{clientId:int}",
            async Task<Results<Ok<List<DeliveryNoteResponse>>, NotFound>> (
            IMediator mediator,
            int clientId,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUninvoicedDeliveryNotesQuery(clientId);

            var result = await mediator.Send(query, cancellationToken);

            return TypedResults.Ok(result.Value);
        });
    }
}
