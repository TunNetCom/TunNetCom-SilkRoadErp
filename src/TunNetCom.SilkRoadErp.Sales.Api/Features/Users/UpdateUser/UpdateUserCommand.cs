using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.UpdateUser;

public record UpdateUserCommand(int UserId, UpdateUserRequest Request) : IRequest<Result>;


