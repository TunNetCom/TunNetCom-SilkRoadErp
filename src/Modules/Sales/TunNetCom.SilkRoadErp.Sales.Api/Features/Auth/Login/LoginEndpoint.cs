using Carter;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Auth;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Auth.Login;

public class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/auth/login", HandleLoginAsync)
            .WithTags(EndpointTags.Auth)
            .WithName("Login")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();
    }

    public async Task<IResult> HandleLoginAsync(
        [FromServices] IMediator mediator,
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var query = new LoginQuery(request);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result.Value);
    }
}

