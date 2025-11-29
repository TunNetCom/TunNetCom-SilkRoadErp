using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.ChangePassword;

public class ChangePasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/users/{id:int}/change-password", async (
                int id,
                ChangePasswordRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new ChangePasswordCommand(id, request);
                var result = await mediator.Send(command, cancellationToken);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { errors = result.Errors.Select(e => e.Message) });
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Changes a user's password.")
            .RequireAuthorization($"Permission:{Permissions.UpdateUser}")
            .WithTags("Users");
    }
}


