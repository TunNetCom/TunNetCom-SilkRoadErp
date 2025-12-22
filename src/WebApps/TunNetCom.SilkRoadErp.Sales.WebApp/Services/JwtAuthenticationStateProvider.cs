using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;
    private bool _hasLoadedFromStorage = false;

    public JwtAuthenticationStateProvider(
        IAuthService authService,
        ILogger<JwtAuthenticationStateProvider> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Always load from localStorage on first call for this circuit
            // This ensures each new browser/device starts with its own localStorage state
            var token = _authService.AccessToken;
            
            if (!_hasLoadedFromStorage)
            {
                _logger.LogInformation("First auth check for this circuit - loading from localStorage");
                await _authService.LoadTokenFromStorageAsync();
                token = _authService.AccessToken;
                _hasLoadedFromStorage = true;
            }
            
            // If still no token after loading from storage, try one more time
            // This handles race conditions with JS interop
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogDebug("No token in memory, attempting to load from storage");
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
            
            // AUTO-LOGOUT DISABLED: No expiration check - token is always considered valid if it exists
            // This prevents automatic disconnections

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
        // Reset the flag so next check will reload from storage
        _hasLoadedFromStorage = false;
        _logger.LogInformation("JwtAuthenticationStateProvider: Authentication state change notified, will reload from storage on next check");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

