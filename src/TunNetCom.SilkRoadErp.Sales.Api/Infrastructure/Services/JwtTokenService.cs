using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;
    private readonly string _issuer;
    private readonly string _audience;
    
    // Default to 240 minutes (4 hours) if not configured
    private const int DefaultAccessTokenExpirationMinutes = 240;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        var secretKey = _configuration["JwtSettings:SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        _issuer = _configuration["JwtSettings:Issuer"] ?? "SilkRoadErp";
        _audience = _configuration["JwtSettings:Audience"] ?? "SilkRoadErp";
        
        // Read from configuration with default fallback
        AccessTokenExpirationMinutes = _configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes", DefaultAccessTokenExpirationMinutes);
    }

    /// <inheritdoc />
    public int AccessTokenExpirationMinutes { get; }

    public string GenerateAccessToken(User user, IList<string> roles, IList<string> permissions)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permissions
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        // Token expiration disabled for simple auth - tokens never expire
        DateTime? expires = null;

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _key,
            ValidateLifetime = false, // We want to validate expired tokens
            ValidIssuer = _issuer,
            ValidAudience = _audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId)
    {
        // This will be implemented in the handler that has access to the database
        // For now, we just return true - the actual validation will be done in the handler
        await Task.CompletedTask;
        return true;
    }
}

