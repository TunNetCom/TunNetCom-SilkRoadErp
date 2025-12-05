using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.MarkNotificationAsRead;

public class MarkNotificationAsReadEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/notifications/{notificationId}/mark-read", async (
                int notificationId,
                [FromQuery] int? userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new MarkNotificationAsReadCommand(notificationId, userId);
                await mediator.Send(command, cancellationToken);
                return Results.NoContent();
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithDescription("Marks a notification as read.")
            .RequireAuthorization()
            .WithTags("Notifications")
            .WithOpenApi();
    }
}

