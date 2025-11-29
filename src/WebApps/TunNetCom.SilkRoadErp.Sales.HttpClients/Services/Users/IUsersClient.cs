using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Users;

public interface IUsersClient
{
    Task<PagedList<UserResponse>> GetUsersAsync(int pageNumber, int pageSize, string? searchKeyword = null, CancellationToken cancellationToken = default);
    Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(int id, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(int id, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<List<RoleResponse>> GetAvailableRolesAsync(CancellationToken cancellationToken = default);
}


