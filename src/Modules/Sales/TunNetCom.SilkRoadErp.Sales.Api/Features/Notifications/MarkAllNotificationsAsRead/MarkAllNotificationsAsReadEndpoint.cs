using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/notifications/mark-all-read", async (
                [FromQuery] int? userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new MarkAllNotificationsAsReadCommand(userId);
                await mediator.Send(command, cancellationToken);
                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Marks all notifications as read.")
            .RequireAuthorization()
            .WithTags("Notifications")
            .WithOpenApi();
    }
}

