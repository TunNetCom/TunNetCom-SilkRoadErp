using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.DeleteUser;

public record DeleteUserCommand(int UserId) : IRequest<Result>;


