using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Notifications;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.GetNotifications;

public class GetNotificationsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/notifications", async (
                [FromQuery] bool? unreadOnly,
                [FromQuery] int? userId,
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetNotificationsQuery(
                    unreadOnly,
                    userId,
                    pageNumber,
                    pageSize);

                var pagedNotifications = await mediator.Send(query, cancellationToken);
                return Results.Ok(pagedNotifications);
            })
            .Produces<PagedList<NotificationResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Retrieves a paginated list of notifications.")
            .RequireAuthorization()
            .WithTags("Notifications")
            .WithOpenApi();
    }
}

