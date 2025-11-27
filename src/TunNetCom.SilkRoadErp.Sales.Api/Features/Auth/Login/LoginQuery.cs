using TunNetCom.SilkRoadErp.Sales.Contracts.Auth;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Auth.Login;

public record LoginQuery(LoginRequest Request) : IRequest<Result<LoginResponse>>;

