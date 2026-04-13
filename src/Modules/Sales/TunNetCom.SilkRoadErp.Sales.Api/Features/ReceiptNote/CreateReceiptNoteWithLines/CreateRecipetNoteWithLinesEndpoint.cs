using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteWithLines;

public class CreateRecipetNoteWithLinesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/receipt_note_with_lines",
            async ([FromBody] CreateReceiptNoteWithLinesRequest request,
            IMediator mediator) =>
        {

            var validationResults = new CreateReceiptNoteWithLinesValidator().Validate(request);



            if (!validationResults.IsValid)
            {
                return Results.BadRequest(new { Errors = validationResults.Errors.ToList() });
            }

        var receiptNoteLines = request.ReceiptNoteLines.Select(x
            => new ReceiptNoteLignes
                (
                    ProductRef: x.ProductRef,
                    ProductDescription: x.ProductDescription,
                    Quantity: x.Quantity,
                    UnitPrice: x.UnitPrice,
                    Discount: x.Discount,
                    Tax: x.Tax
                    )).ToList();

            var command = new CreateReceiptNoteWithLigneCommand(
                NumBonFournisseur: request.NumBonFournisseur,
                DateLivraison: request.DateLivraison,
                IdFournisseur: request.IdFournisseur,
                Date: request.Date,
                NumFactureFournisseur: request.NumFactureFournisseur,
                ReceiptNoteLines: receiptNoteLines);

            var result = await mediator.Send(command);
            if (result.IsFailed)
            {
                return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
            }

            return Results.Created($"/receipt_note_with_lines", result.Value);

        })
        .WithTags(EndpointTags.ReceiptNotes)
        .WithName("CreateReceiptNoteWithLines")
        .WithSummary("Create a new receipt note with lines")
        .Produces<List<int>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
