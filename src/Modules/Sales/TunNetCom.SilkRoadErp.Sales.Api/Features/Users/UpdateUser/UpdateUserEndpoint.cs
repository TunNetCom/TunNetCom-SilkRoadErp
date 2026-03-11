using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.UpdateUser;

public class UpdateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/users/{id:int}", async (
                int id,
                UpdateUserRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateUserCommand(id, request);
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { errors = result.Errors.Select(e => e.Message) });
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Updates an existing user.")
            .RequireAuthorization($"Permission:{Permissions.UpdateUser}")
            .WithTags("Users");
    }
}


