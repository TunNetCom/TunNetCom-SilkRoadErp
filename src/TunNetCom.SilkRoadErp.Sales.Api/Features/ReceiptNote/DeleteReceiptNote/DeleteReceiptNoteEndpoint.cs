using TunNetCom.SilkRoadErp.Sales.Api.Features.Customers.DeleteCustomer;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DeleteReceiptNote;

public class DeleteReceiptNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/receiptnotes/{num:int}", async Task<Results<NoContent, NotFound>> (IMediator mediator, int num, CancellationToken cancellationToken) =>
        {
            var deleteReceiptNoteCommand = new DeleteReceiptNoteCommand(num);
            var deleteResult = await mediator.Send(deleteReceiptNoteCommand, cancellationToken);
            if (deleteResult.IsFailed)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        });
    }
}
