using TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.CreateReceiptNote;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.UpdateReceiptNoteWithLines;

public class UpdateReceiptNoteWithLinesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/receiptnotes/{num:int}/with-lines",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator,
                int num,
                CreateReceiptNoteWithLinesRequest request,
                CancellationToken cancellationToken) =>
            {
                var updateCommand = new UpdateReceiptNoteWithLinesCommand(
                    Num: num,
                    NumBonFournisseur: request.NumBonFournisseur,
                    DateLivraison: request.DateLivraison,
                    IdFournisseur: request.IdFournisseur,
                    Date: request.Date,
                    NumFactureFournisseur: request.NumFactureFournisseur,
                    ReceiptNoteLines: request.ReceiptNoteLines.Select(x => new ReceiptNoteLignes(
                        ProductRef: x.ProductRef,
                        ProductDescription: x.ProductDescription,
                        Quantity: x.Quantity,
                        UnitPrice: x.UnitPrice,
                        Discount: x.Discount,
                        Tax: x.Tax
                    )).ToList()
                );

                var result = await mediator.Send(updateCommand, cancellationToken);

                if (result.IsEntityNotFound())
                {
                    return TypedResults.NotFound();
                }

                if (result.IsFailed)
                {
                    return result.ToValidationProblem();
                }

                return TypedResults.NoContent();
            })
            .WithTags(EndpointTags.ReceiptNotes)
            .WithName("UpdateReceiptNoteWithLines")
            .WithSummary("Update a receipt note with its lines");
    }
}

