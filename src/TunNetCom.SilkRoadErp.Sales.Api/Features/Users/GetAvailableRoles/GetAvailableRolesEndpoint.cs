using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.GetAvailableRoles;

public class GetAvailableRolesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/users/roles", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetAvailableRolesQuery();
                var roles = await mediator.Send(query, cancellationToken);
                return Results.Ok(roles);
            })
            .Produces<List<RoleResponse>>(StatusCodes.Status200OK)
            .WithDescription("Retrieves all available roles.")
            .RequireAuthorization($"Permission:{Permissions.ViewRoles}")
            .WithTags("Users");
    }
}


