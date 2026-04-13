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
        // AUTO-LOGOUT DISABLED: Do nothing - no logout, no redirect
        _logger.LogWarning("AutoLogoutService: HandleTokenExpirationAsync called but auto-logout is disabled - doing nothing");
        await Task.CompletedTask;
    }

    private async Task ForceLogoutAndNavigateAsync()
    {
        // AUTO-LOGOUT DISABLED: Do nothing - no logout, no redirect
        _logger.LogWarning("AutoLogoutService: ForceLogoutAndNavigateAsync called but auto-logout is disabled - doing nothing");
        await Task.CompletedTask;
    }
}

