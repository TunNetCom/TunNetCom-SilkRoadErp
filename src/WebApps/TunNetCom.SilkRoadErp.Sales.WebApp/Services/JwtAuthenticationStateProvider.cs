using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;
    private readonly IAutoLogoutService _autoLogoutService;
    private DateTime _lastTokenLoadTime = DateTime.MinValue;
    private readonly TimeSpan _tokenLoadCacheDuration = TimeSpan.FromMinutes(5); // Cache token load for 5 minutes

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
            // Only load from storage if token is not already in memory AND we haven't loaded recently
            // This optimization prevents frequent JS interop calls that cause page refreshes
            var token = _authService.AccessToken;
            var timeSinceLastLoad = DateTime.UtcNow - _lastTokenLoadTime;
            
            if (string.IsNullOrEmpty(token) && timeSinceLastLoad > _tokenLoadCacheDuration)
            {
                // Only try to load from storage if token is not in memory and cache expired
                _lastTokenLoadTime = DateTime.UtcNow;
                await _authService.LoadTokenFromStorageAsync();
                token = _authService.AccessToken;
            }

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
            
            // Buffer increased to 5 minutes to match 4-hour token expiration
            // This prevents false positives where a token is considered expired immediately after login
            var expirationBuffer = TimeSpan.FromMinutes(5);
            var timeUntilExpiration = jwtToken.ValidTo - DateTime.UtcNow;
            
            // Check if token is expired (accounting for buffer)
            if (timeUntilExpiration <= expirationBuffer)
            {
                // Token is expired or will expire very soon
                if (timeUntilExpiration <= TimeSpan.Zero)
                {
                    _logger.LogWarning("Token has expired. ValidTo: {ValidTo}, Current: {Current}, TimeSinceExpiration: {TimeSinceExpiration}", 
                        jwtToken.ValidTo, DateTime.UtcNow, -timeUntilExpiration);
                    
                    // Only handle expiration if token is significantly expired (more than 5 seconds)
                    // This prevents immediate logout right after login due to timing issues
                    if (timeUntilExpiration <= TimeSpan.FromSeconds(-5))
                    {
                        _logger.LogWarning("Token expired {TimeSinceExpiration} ago, handling expiration", -timeUntilExpiration);
                        
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
                    else
                    {
                        _logger.LogInformation("Token expired but within grace period ({TimeSinceExpiration}), allowing authentication", -timeUntilExpiration);
                    }
                }
                else
                {
                    // Token is about to expire soon (within buffer), but still valid
                    _logger.LogDebug("Token will expire soon ({TimeUntilExpiration}), but still valid", timeUntilExpiration);
                }
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

