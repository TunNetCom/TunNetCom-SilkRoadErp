using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.GetUserById;

public class GetUserByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/users/{id:int}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUserByIdQuery(id);
                var result = await mediator.Send(query, cancellationToken);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(new { error = result.Errors.FirstOrDefault()?.Message });
            })
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Retrieves a user by ID.")
            .RequireAuthorization($"Permission:{Permissions.ViewUsers}")
            .WithTags("Users");
    }
}


