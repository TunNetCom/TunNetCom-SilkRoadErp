namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.UpdateReceiptNote;

public class UpdateReceiptNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/receiptnotes/{num:int}",
            async Task<Results<NoContent, NotFound, ValidationProblem>> (
                IMediator mediator, int num,
                UpdateReceiptNoteRequest request,
                CancellationToken cancellationToken) =>
            {
                var updateReceiptNoteCommand = new UpdateReceiptNoteCommand(
                    Num: request.Num,
                    NumBonFournisseur: request.NumBonFournisseur,
                    DateLivraison: request.DateLivraison,
                    IdFournisseur: request.IdFournisseur,
                    Date: request.Date,
                    NumFactureFournisseur: request.NumFactureFournisseur);

                var updateResult = await mediator.Send(updateReceiptNoteCommand, cancellationToken);

                if (updateResult.IsEntityNotFound())
                {
                    return TypedResults.NotFound();
                }

                if (updateResult.IsFailed)
                {
                    return updateResult.ToValidationProblem();
                }

                return TypedResults.NoContent();
            });
    }
}
