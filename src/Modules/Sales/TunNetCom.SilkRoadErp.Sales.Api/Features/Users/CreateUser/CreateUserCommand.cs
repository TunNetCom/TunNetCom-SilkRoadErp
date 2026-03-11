using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.CreateUser;

public record CreateUserCommand(CreateUserRequest Request) : IRequest<Result<int>>;


