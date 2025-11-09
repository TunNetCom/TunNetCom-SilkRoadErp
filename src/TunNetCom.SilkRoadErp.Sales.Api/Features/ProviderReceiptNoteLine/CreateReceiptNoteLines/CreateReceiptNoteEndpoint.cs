using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderReceiptNoteLine.CreateReceiptNoteLines;

public class GetRecepitNotesLinesWithSummariesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/receipt_note_lines",
            async ([FromBody] List<CreateReceiptNoteLineRequest> request,
            IMediator mediator) =>
        {

            var validationResults = request
                .Select(x => new CreateReceiptNoteValidator().Validate(x))
                .ToList();


            if (validationResults.Any(vr => !vr.IsValid))
            {
                var errors = validationResults
                    .SelectMany(vr => vr.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Results.BadRequest(new { Errors = errors });
            }

            var receiptNoteLines = request.Select(x 
                => new ReceiptNoteLignes
                    (
                    RecipetNoteNumber: x.RecipetNoteNumber,
                    ProductRef : x.ProductRef,
                    ProductDescription: x.ProductDescription,
                    Quantity: x.Quantity,
                    UnitPrice: x.UnitPrice,
                    Discount: x.Discount,
                    Tax: x.Tax)).ToList();

            var command = new CreateReceiptNoteLigneCommand(ReceiptNoteLines: receiptNoteLines);

            var result = await mediator.Send(command);
            if (result.IsFailed)
            {
                return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
            }

            return Results.Created($"/receipt_note_lines", result.Value);

        })
        .WithTags("Receipt Note Lines")
        .WithName("CreateReceiptNoteLines")
        .WithSummary("Create a new receipt note lines")
        .Produces<List<int>>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
