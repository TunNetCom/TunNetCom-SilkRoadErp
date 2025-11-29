using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.GetUserById;

public record GetUserByIdQuery(int Id) : IRequest<Result<UserResponse>>;


