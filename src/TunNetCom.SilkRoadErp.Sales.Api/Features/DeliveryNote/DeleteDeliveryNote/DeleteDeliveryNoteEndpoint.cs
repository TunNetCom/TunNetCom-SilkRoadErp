namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeleteDeliveryNote;

public class DeleteDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/deliveryNote/{num:int}", async Task<Results<NoContent, NotFound>> (
            IMediator mediator,
            int num,
            CancellationToken cancellationToken) =>
        {
            var deleteDeliveryNoteCommand = new DeleteDeliveryNoteCommand(num);
            var deleteResult = await mediator.Send(deleteDeliveryNoteCommand, cancellationToken);

            if (deleteResult.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        });
    }
}
