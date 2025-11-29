using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;
    private readonly IAutoLogoutService _autoLogoutService;

    public JwtAuthenticationStateProvider(
        IAuthService authService,
        ILogger<JwtAuthenticationStateProvider> logger,
        IAutoLogoutService autoLogoutService)
    {
        _authService = authService;
        _logger = logger;
        _autoLogoutService = autoLogoutService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Try to load token from storage first
            await _authService.LoadTokenFromStorageAsync();
            
            var token = _authService.AccessToken;

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("No token found, returning anonymous user");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Parse JWT token to extract claims
            var handler = new JwtSecurityTokenHandler();
            
            if (!handler.CanReadToken(token))
            {
                _logger.LogWarning("Token is not a valid JWT token");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var jwtToken = handler.ReadJwtToken(token);
            
            // Check if token is expired
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                _logger.LogWarning("Token has expired. ValidTo: {ValidTo}, Current: {Current}", 
                    jwtToken.ValidTo, DateTime.UtcNow);
                
                // Handle token expiration in background (fire-and-forget)
                // This will try to refresh the token or logout the user
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _autoLogoutService.HandleTokenExpirationAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling token expiration in JwtAuthenticationStateProvider");
                    }
                });
                
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Extract all claims from the JWT token
            var claims = jwtToken.Claims.ToList();
            
            _logger.LogInformation("JWT Authentication: Token parsed successfully with {ClaimCount} claims", claims.Count);
            
            // Log permission claims for debugging
            var permissionClaims = claims.Where(c => c.Type == "permission").ToList();
            _logger.LogInformation("JWT Authentication: Found {PermissionCount} permission claims", permissionClaims.Count);
            
            if (permissionClaims.Count > 0)
            {
                _logger.LogDebug("First 5 permissions: {Permissions}", 
                    string.Join(", ", permissionClaims.Take(5).Select(c => c.Value)));
            }

            // Create ClaimsIdentity with "jwt" authentication type
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("prerendering") || 
            ex.Message.Contains("statically rendered") ||
            ex.Message.Contains("JavaScript interop calls cannot be issued"))
        {
            // During prerendering, JS interop is not available
            _logger.LogDebug("Cannot access token during prerendering");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

