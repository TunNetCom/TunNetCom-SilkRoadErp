using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.GetAvailableRoles;

public class GetAvailableRolesQueryHandler(
    SalesContext _context)
    : IRequestHandler<GetAvailableRolesQuery, List<RoleResponse>>
{
    public async Task<List<RoleResponse>> Handle(GetAvailableRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _context.Role
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            })
            .ToListAsync(cancellationToken);

        return roles;
    }
}


