using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.GetUnreadNotificationCount;

public class GetUnreadNotificationCountEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/notifications/unread-count", async (
                [FromQuery] int? userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUnreadNotificationCountQuery(userId);
                var count = await mediator.Send(query, cancellationToken);
                return Results.Ok(new { count });
            })
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Gets the count of unread notifications.")
            .RequireAuthorization()
            .WithTags("Notifications")
            .WithOpenApi();
    }
}

