using Carter;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Auth.Logout;

public class LogoutEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/auth/logout", HandleLogoutAsync)
            .WithTags(EndpointTags.Auth)
            .WithName("Logout")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    public async Task<IResult> HandleLogoutAsync(
        [FromServices] IMediator mediator,
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LogoutCommand(request.RefreshToken);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return Results.BadRequest();
        }

        return Results.Ok();
    }
}

public class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

