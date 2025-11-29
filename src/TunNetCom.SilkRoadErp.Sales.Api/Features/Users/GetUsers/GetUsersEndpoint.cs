using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.GetUsers;

public class GetUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/users", async (
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromQuery] string? searchKeyword,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        detail: "PageNumber and PageSize must be greater than zero.");
                }

                var query = new GetUsersQuery(pageNumber, pageSize, searchKeyword);
                var pagedUsers = await mediator.Send(query, cancellationToken);
                return Results.Ok(pagedUsers);
            })
            .Produces<PagedList<UserResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithDescription("Retrieves a paginated list of users with optional search filtering.")
            .RequireAuthorization($"Permission:{Permissions.ViewUsers}")
            .WithTags("Users");
    }
}


