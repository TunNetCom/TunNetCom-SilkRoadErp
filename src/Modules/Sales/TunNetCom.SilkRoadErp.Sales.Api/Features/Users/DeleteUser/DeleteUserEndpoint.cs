namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.DeleteUser;

public class DeleteUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/users/{id:int}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteUserCommand(id);
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { errors = result.Errors.Select(e => e.Message) });
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Deactivates a user (soft delete).")
            .RequireAuthorization($"Permission:{Permissions.DeleteUser}")
            .WithTags("Users");
    }
}


