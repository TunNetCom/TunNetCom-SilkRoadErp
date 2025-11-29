using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.GetUsers;

public record GetUsersQuery(
    int PageNumber,
    int PageSize,
    string? SearchKeyword) : IRequest<PagedList<UserResponse>>;


