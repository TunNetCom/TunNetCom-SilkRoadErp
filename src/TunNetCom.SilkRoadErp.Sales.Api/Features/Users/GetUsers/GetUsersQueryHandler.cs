using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.GetUsers;

public class GetUsersQueryHandler(
    SalesContext _context,
    ILogger<GetUsersQueryHandler> _logger)
    : IRequestHandler<GetUsersQuery, PagedList<UserResponse>>
{
    public async Task<PagedList<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching users with PageNumber={PageNumber}, PageSize={PageSize}, Search={Search}", 
            request.PageNumber, request.PageSize, request.SearchKeyword);

        var query = _context.User
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsNoTracking()
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchKeyword))
        {
            query = query.Where(u => 
                u.Username.Contains(request.SearchKeyword) ||
                u.Email.Contains(request.SearchKeyword) ||
                (u.FirstName != null && u.FirstName.Contains(request.SearchKeyword)) ||
                (u.LastName != null && u.LastName.Contains(request.SearchKeyword)));
        }

        // Order by username
        query = query.OrderBy(u => u.Username);

        // Project to response DTO
        var usersQuery = query.Select(u => new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        });

        var pagedUsers = await PagedList<UserResponse>.ToPagedListAsync(
            usersQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        _logger.LogInformation("Retrieved {Count} users", pagedUsers.Items.Count);

        return pagedUsers;
    }
}


