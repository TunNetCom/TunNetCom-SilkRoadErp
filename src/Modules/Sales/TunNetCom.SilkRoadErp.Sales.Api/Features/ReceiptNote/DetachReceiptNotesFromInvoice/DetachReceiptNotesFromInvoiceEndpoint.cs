namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DetachReceiptNotesFromInvoice;

public class DetachReceiptNotesFromInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut(
                "/receipt-note/detach",
                async Task<IResult> (
                    IMediator mediator,
                    [FromBody] DetachReceiptNotesFromInvoiceCommand command,
                    CancellationToken cancellationToken) =>
                {
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
            .WithName("DetachReceiptNotesFromInvoice")
            .WithTags(EndpointTags.ReceiptNotes)
            .Produces<ReceiptNoteResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest, "application/problem+json")
            .ProducesProblem(StatusCodes.Status500InternalServerError, "application/problem+json")
            .WithDescription("Detaches receipt notes from an invoice.");
    }
}