using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesByClientId;

public class GetDeliveryNotesByClientIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet(
            "/deliveryNote/client/{clientId:int}",
            async Task<Results<Ok<List<DeliveryNoteResponse>>, NotFound>> (
            IMediator mediator,
            int clientId,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDeliveryNoteByClientIdQuery(clientId);

            var result = await mediator.Send(query, cancellationToken);

            if (result.IsEntityNotFound())
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(result.Value);
        })
        .WithTags(EndpointTags.DeliveryNotes);
    }
}
