using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class AutoLogoutService : IAutoLogoutService
{
    private readonly IAuthService _authService;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<AutoLogoutService> _logger;
    private readonly ITokenExpirationNotifier _tokenExpirationNotifier;
    private readonly SemaphoreSlim _logoutSemaphore = new(1, 1);
    private bool _isLoggingOut = false;

    public AutoLogoutService(
        IAuthService authService,
        NavigationManager navigationManager,
        IJSRuntime jsRuntime,
        ILogger<AutoLogoutService> logger,
        ITokenExpirationNotifier tokenExpirationNotifier)
    {
        _authService = authService;
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
        _logger = logger;
        _tokenExpirationNotifier = tokenExpirationNotifier;
    }

    public async Task HandleTokenExpirationAsync()
    {
        // Éviter les déconnexions multiples simultanées
        if (_isLoggingOut)
        {
            _logger.LogWarning("AutoLogoutService: Already processing logout, skipping");
            return;
        }

        // Essayer d'acquérir le verrou (non-bloquant)
        if (!await _logoutSemaphore.WaitAsync(0))
        {
            _logger.LogWarning("AutoLogoutService: Another logout is in progress, skipping");
            return;
        }

        try
        {
            _isLoggingOut = true;
            _logger.LogWarning("AutoLogoutService: ===== TOKEN EXPIRED - Starting refresh attempt =====");

            // Essayer de rafraîchir le token
            _logger.LogInformation("AutoLogoutService: Calling RefreshTokenAsync...");
            var refreshed = await _authService.RefreshTokenAsync();
            _logger.LogInformation("AutoLogoutService: RefreshTokenAsync returned: {Refreshed}", refreshed);

            if (!refreshed)
            {
                // Refresh échoué, déconnecter immédiatement
                _logger.LogError("AutoLogoutService: ===== TOKEN REFRESH FAILED - FORCING LOGOUT =====");
                
                // Notify components first (they can handle navigation in Blazor context)
                try
                {
                    await _tokenExpirationNotifier.NotifyTokenExpiredAsync();
                }
                catch (Exception notifyEx)
                {
                    _logger.LogWarning(notifyEx, "AutoLogoutService: Error notifying components");
                }
                
                await ForceLogoutAndNavigateAsync();
            }
            else
            {
                _logger.LogInformation("AutoLogoutService: ===== TOKEN REFRESHED SUCCESSFULLY =====");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AutoLogoutService: ===== EXCEPTION during token expiration handling - FORCING LOGOUT =====");
            // En cas d'erreur, déconnecter pour être sûr
            await ForceLogoutAndNavigateAsync();
        }
        finally
        {
            _isLoggingOut = false;
            _logoutSemaphore.Release();
            _logger.LogInformation("AutoLogoutService: ===== HandleTokenExpirationAsync completed =====");
        }
    }

    private async Task ForceLogoutAndNavigateAsync()
    {
        try
        {
            _logger.LogWarning("AutoLogoutService: ===== FORCE LOGOUT STARTED =====");
            
            // Déconnecter d'abord (nettoie les tokens)
            _logger.LogInformation("AutoLogoutService: Calling LogoutAsync...");
            await _authService.LogoutAsync();
            _logger.LogInformation("AutoLogoutService: LogoutAsync completed");
            
            // Notify components to handle navigation (they are in Blazor context)
            _logger.LogInformation("AutoLogoutService: Notifying components of token expiration");
            try
            {
                await _tokenExpirationNotifier.NotifyTokenExpiredAsync();
                _logger.LogInformation("AutoLogoutService: Components notified");
            }
            catch (Exception notifyEx)
            {
                _logger.LogWarning(notifyEx, "AutoLogoutService: Error notifying components, will try direct navigation");
                
                // Fallback: try direct navigation only if notification fails
                try
                {
                    _navigationManager.NavigateTo("/login", forceLoad: true);
                    _logger.LogInformation("AutoLogoutService: Direct navigation attempted");
                }
                catch (Exception navEx)
                {
                    _logger.LogError(navEx, "AutoLogoutService: Direct navigation also failed");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AutoLogoutService: ===== ERROR DURING FORCE LOGOUT =====");
        }
    }
}

