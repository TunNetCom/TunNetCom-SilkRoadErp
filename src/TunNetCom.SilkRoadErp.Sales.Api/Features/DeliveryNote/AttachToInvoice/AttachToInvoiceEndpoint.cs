using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;


namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.AttachToInvoice;

public class AttachToInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/deliveryNote/attachToInvoice",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator,
                [FromBody] AttachToInvoiceRequest attachToInvoiceRequest,
                CancellationToken cancellationToken) =>
            {
                return await HandleAttachToInvoiceAsync(mediator, attachToInvoiceRequest, cancellationToken);
            })
            .WithTags(SwaggerTags.DeliveryNotes);
    }

    // ✅ Méthode publique à appeler depuis le test
    public async Task<Results<NoContent, NotFound, ValidationProblem>> HandleAttachToInvoiceAsync(
        IMediator mediator,
        AttachToInvoiceRequest attachToInvoiceRequest,
        CancellationToken cancellationToken)
    {
        var command = new AttachToInvoiceCommand(attachToInvoiceRequest.InvoiceId, attachToInvoiceRequest.DeliveryNoteIds);
        var attachToInvoiceResult = await mediator.Send(command, cancellationToken);

        if (attachToInvoiceResult.HasError<EntityNotFound>())
        {
            return TypedResults.NotFound();
        }

        if (attachToInvoiceResult.IsFailed)
        {
            return attachToInvoiceResult.ToValidationProblem();
        }

        return TypedResults.NoContent();
    }
}
