namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeleteDeliveryNote;

public class DeleteDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/deliveryNote/{num:int}", HandleDeleteDeliveryNoteAsync)
            .WithTags(EndpointTags.DeliveryNotes);
    }

    // 🧪 Cette méthode publique est testable
    public static async Task<Results<NoContent, NotFound>> HandleDeleteDeliveryNoteAsync(
        IMediator mediator,
        int num,
        CancellationToken cancellationToken)
    {
        var deleteDeliveryNoteCommand = new DeleteDeliveryNoteCommand(num);
        var deleteResult = await mediator.Send(deleteDeliveryNoteCommand, cancellationToken);

        if (deleteResult.IsEntityNotFound())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }
}
