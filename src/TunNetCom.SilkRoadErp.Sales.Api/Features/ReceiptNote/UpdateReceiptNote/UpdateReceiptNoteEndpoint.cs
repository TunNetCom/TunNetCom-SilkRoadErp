namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.UpdateReceiptNote;

public class UpdateReceiptNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/receiptnotes/{num:int}",
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
            NumFactureFournisseur: request.NumFactureFournisseur
            );
                var Result = await mediator.Send(updateReceiptNoteCommand, cancellationToken);

                if (Result.HasError<EntityNotFound>())
                {
                    return TypedResults.NotFound();
                }

                if (Result.IsFailed)
                {
                    return Result.ToValidationProblem();
                }

                return TypedResults.NoContent();
            });
    }
}
