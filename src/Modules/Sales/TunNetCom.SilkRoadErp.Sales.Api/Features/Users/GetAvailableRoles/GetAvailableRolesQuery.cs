using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.GetAvailableRoles;

public record GetAvailableRolesQuery : IRequest<List<RoleResponse>>;


