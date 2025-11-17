namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.AttachReceiptNotesToInvoice;

public class AttachReceiptNotesToInvoiceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
        _ = app.MapPut("/receipt-note/attach-to-invoice",
                async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator,
                [FromBody] AttachReceiptNotesToInvoiceCommand command,
                CancellationToken cancellationToken) =>
                {
                    var attachCommand = new AttachReceiptNotesToInvoiceCommand(
                        command.ReceiptNotesIds,
                        command.InvoiceId);

                    var result = await mediator.Send(attachCommand, cancellationToken);
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
            .WithName("AttachReceiptNotesToInvoice")
            .WithTags(SwaggerTags.ReceiptNotes)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
            .WithDescription("Attaches receipt notes to an invoice.");
        }
    }
