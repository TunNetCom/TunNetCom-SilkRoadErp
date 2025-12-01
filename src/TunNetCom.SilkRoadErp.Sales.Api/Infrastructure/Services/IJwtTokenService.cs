namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, IList<string> roles, IList<string> permissions);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId);
}

