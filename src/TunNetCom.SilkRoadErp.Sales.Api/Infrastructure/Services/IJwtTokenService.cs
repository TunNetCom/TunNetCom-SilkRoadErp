namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public interface IJwtTokenService
{
    /// <summary>
    /// Gets the access token expiration time in minutes (from configuration)
    /// </summary>
    int AccessTokenExpirationMinutes { get; }
    
    string GenerateAccessToken(User user, IList<string> roles, IList<string> permissions);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId);
}

