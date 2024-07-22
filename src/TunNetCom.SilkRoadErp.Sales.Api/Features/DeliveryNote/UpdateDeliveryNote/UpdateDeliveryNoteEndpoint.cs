namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.UpdateDeliveryNote;

public class UpdateDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/deliveryNote/{num:int}",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator, int num,
                UpdateDeliveryNoteRequest updateDeliveryNoteRequest,
                CancellationToken cancellationToken) =>
            {
            var updateDeliveryNoteCommand = new UpdateDeliveryNoteCommand(
                Num: num,
                Date: updateDeliveryNoteRequest.Date,
                TotHTva: updateDeliveryNoteRequest.TotHTva,
                TotTva: updateDeliveryNoteRequest.TotTva,
                NetPayer: updateDeliveryNoteRequest.NetPayer,
                TempBl: updateDeliveryNoteRequest.TempBl,
                NumFacture: updateDeliveryNoteRequest.NumFacture,
                ClientId: updateDeliveryNoteRequest.ClientId);

                var updateDeliveryNoteResult = await mediator.Send(updateDeliveryNoteCommand, cancellationToken);

                if (updateDeliveryNoteResult.HasError<EntityNotFound>())
                {
                    return TypedResults.NotFound();
                }

                if (updateDeliveryNoteResult.IsFailed)
                {
                    return updateDeliveryNoteResult.ToValidationProblem();
                }

                return TypedResults.NoContent();
            });
    }
}
