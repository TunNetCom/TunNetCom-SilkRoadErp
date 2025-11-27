using Carter;
using Microsoft.AspNetCore.Mvc;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Constants;
using TunNetCom.SilkRoadErp.Sales.Contracts.Auth;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Auth.RefreshToken;

public class RefreshTokenEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/auth/refresh", HandleRefreshTokenAsync)
            .WithTags(EndpointTags.Auth)
            .WithName("RefreshToken")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();
    }

    public async Task<IResult> HandleRefreshTokenAsync(
        [FromServices] IMediator mediator,
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var query = new RefreshTokenQuery(request);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(result.Value);
    }
}

