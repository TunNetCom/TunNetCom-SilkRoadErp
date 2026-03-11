using TunNetCom.SilkRoadErp.Sales.Contracts.Auth;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Auth.RefreshToken;

public record RefreshTokenQuery(RefreshTokenRequest Request) : IRequest<Result<LoginResponse>>;

