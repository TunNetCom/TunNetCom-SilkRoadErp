using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DetachFromInvoice;

public class DetachFromInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/deliveryNote/detachFromInvoice",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator,
                [FromBody] DetachFromInvoiceRequest request,
                CancellationToken cancellationToken) =>
            {
                var command = new DetachFromInvoiceCommand(request.InvoiceId, request.DeliveryNoteIds);
                var result = await mediator.Send(command, cancellationToken);

                if (result.HasError<EntityNotFound>())
                {
                    return TypedResults.NotFound();
                }

                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }

                return TypedResults.NoContent();
            })
            .WithTags(EndpointTags.DeliveryNotes);
    }
}