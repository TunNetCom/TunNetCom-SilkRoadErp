
namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.AttachToInvoice;

public class AttachToInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/deliveryNote/attachToInvoice",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator,
                [FromBody] AttachToInvoiceRequest attachToInvoiceRequest,
                CancellationToken cancellationToken) =>
            {
                var command = new AttachToInvoiceCommand(attachToInvoiceRequest.InvoiceId, attachToInvoiceRequest.DeliveryNoteIds);
                var attachToInvoiceResult = await mediator.Send(command);

                if (attachToInvoiceResult.HasError<EntityNotFound>())
                {
                    return TypedResults.NotFound();
                }

                if (attachToInvoiceResult.IsFailed)
                {
                    return attachToInvoiceResult.ToValidationProblem();
                }
                return TypedResults.NoContent();
            });
    }
}
