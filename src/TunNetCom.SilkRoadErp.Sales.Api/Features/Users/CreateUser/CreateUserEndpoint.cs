using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.CreateUser;

public class CreateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/users", async (
                CreateUserRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateUserCommand(request);
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess
                    ? Results.Created($"/users/{result.Value}", new { id = result.Value })
                    : Results.BadRequest(new { errors = result.Errors.Select(e => e.Message) });
            })
            .Produces<object>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Creates a new user.")
            .RequireAuthorization($"Permission:{Permissions.CreateUser}")
            .WithTags("Users");
    }
}


