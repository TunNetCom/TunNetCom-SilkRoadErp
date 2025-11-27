namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Auth.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Result>;

