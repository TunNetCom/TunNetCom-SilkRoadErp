using Carter;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeleteDeliveryNote;

public class DeleteDeliveryNoteEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/deliveryNote/{num:int}", HandleDeleteDeliveryNoteAsync);
    }

    // 🧪 Cette méthode publique est testable
    public static async Task<Results<NoContent, NotFound>> HandleDeleteDeliveryNoteAsync(
        IMediator mediator,
        int num,
        CancellationToken cancellationToken)
    {
        var deleteDeliveryNoteCommand = new DeleteDeliveryNoteCommand(num);
        var deleteResult = await mediator.Send(deleteDeliveryNoteCommand, cancellationToken);

        if (deleteResult.IsEntityNotFound())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }
}
