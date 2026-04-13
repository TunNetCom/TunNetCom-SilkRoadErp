using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.Users;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Users.ChangePassword;

public record ChangePasswordCommand(int UserId, ChangePasswordRequest Request) : IRequest<Result>;


