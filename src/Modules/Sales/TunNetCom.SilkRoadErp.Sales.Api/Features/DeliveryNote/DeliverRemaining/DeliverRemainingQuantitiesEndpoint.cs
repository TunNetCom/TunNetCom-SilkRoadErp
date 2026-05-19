using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeliverRemaining;

public class DeliverRemainingQuantitiesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/delivery-notes/deliver-remaining", async (
            DeliverRemainingQuantitiesCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsFailed)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(result.Value);
        })
        .WithTags("DeliveryNotes")
        .WithName("DeliverRemainingQuantities");
    }
}
