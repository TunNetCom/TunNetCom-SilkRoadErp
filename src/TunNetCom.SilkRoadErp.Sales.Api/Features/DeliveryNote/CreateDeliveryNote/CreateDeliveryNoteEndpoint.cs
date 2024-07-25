namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.CreateDeliveryNote;

public class CreateDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) 
    {
        app.MapPost(
            "/deliveryNote",
            async Task<Results<Created<CreateDeliveryNoteRequest>, ValidationProblem>> (
                IMediator mediator,
                CreateDeliveryNoteRequest createDeliveryNoteRequest,
                CancellationToken cancellationToken) =>
            {
                var createDeliveryNoteCommand = new CreateDeliveryNoteCommand
                (
                    Date: createDeliveryNoteRequest.Date,
                    TotHTva: createDeliveryNoteRequest.TotHTva,
                    TotTva : createDeliveryNoteRequest.TotTva,
                    NetPayer : createDeliveryNoteRequest.NetPayer,
                    TempBl: createDeliveryNoteRequest.TempBl,
                    NumFacture : createDeliveryNoteRequest.NumFacture,
                    ClientId : createDeliveryNoteRequest.ClientId,
                    Lignes : createDeliveryNoteRequest.Lignes
                );

                var result = await mediator.Send(createDeliveryNoteCommand, cancellationToken);
                
                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }

                return TypedResults.Created($"/deliveryNote/{result.Value}", createDeliveryNoteRequest);
            }
            );
    }
}
