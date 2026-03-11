using Microsoft.AspNetCore.Components.Authorization;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

/// <summary>
/// Service pour vérifier les permissions de l'utilisateur.
/// Subscribes to authentication state changes to invalidate permission cache automatically.
/// </summary>
public class PermissionService : IPermissionService, IDisposable
{
    private readonly IAuthService _authService;
    private readonly ILogger<PermissionService> _logger;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private IReadOnlyList<string>? _cachedPermissions;
    private DateTime _cacheExpiration = DateTime.MinValue;
    private string? _cachedUserName; // Track username to detect user changes
    
    // Reduced cache duration to 1 minute for faster response to auth changes
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public PermissionService(
        IAuthService authService,
        ILogger<PermissionService> logger,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _authService = authService;
        _logger = logger;
        _authenticationStateProvider = authenticationStateProvider;
        
        // Subscribe to authentication state changes to invalidate cache
        _authenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    public void Dispose()
    {
        // Unsubscribe from authentication state changes
        _authenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        // Invalidate the permission cache when authentication state changes
        _logger.LogInformation("PermissionService: Authentication state changed, invalidating permission cache");
        _cachedPermissions = null;
        _cacheExpiration = DateTime.MinValue;
        _cachedUserName = null;
    }

    public async Task<bool> HasPermissionAsync(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            _logger.LogWarning("HasPermissionAsync called with null or empty permission");
            return false;
        }

        var permissions = await GetUserPermissionsAsync();
        var hasPermission = permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        
        _logger.LogDebug("Permission check: {Permission} = {HasPermission}", permission, hasPermission);
        return hasPermission;
    }

    public async Task<bool> HasAnyPermissionAsync(params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
        {
            _logger.LogWarning("HasAnyPermissionAsync called with null or empty permissions array");
            return false;
        }

        var userPermissions = await GetUserPermissionsAsync();
        var hasAny = permissions.Any(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase));
        
        _logger.LogDebug("HasAnyPermission check: {Permissions} = {HasAny}", string.Join(", ", permissions), hasAny);
        return hasAny;
    }

    public async Task<bool> HasAllPermissionsAsync(params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
        {
            _logger.LogWarning("HasAllPermissionsAsync called with null or empty permissions array");
            return false;
        }

        var userPermissions = await GetUserPermissionsAsync();
        var hasAll = permissions.All(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase));
        
        _logger.LogDebug("HasAllPermissions check: {Permissions} = {HasAll}", string.Join(", ", permissions), hasAll);
        return hasAll;
    }

    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync()
    {
        try
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var currentUserName = user?.Identity?.Name;

            // Check if user changed - invalidate cache if different user
            if (_cachedUserName != null && _cachedUserName != currentUserName)
            {
                _logger.LogInformation("PermissionService: User changed from {OldUser} to {NewUser}, invalidating cache",
                    _cachedUserName, currentUserName ?? "anonymous");
                _cachedPermissions = null;
                _cacheExpiration = DateTime.MinValue;
            }

            // Check cache validity
            if (_cachedPermissions != null && DateTime.UtcNow < _cacheExpiration)
            {
                _logger.LogDebug("Returning cached permissions. Count: {Count}", _cachedPermissions.Count);
                return _cachedPermissions;
            }

            if (user?.Identity?.IsAuthenticated != true)
            {
                _logger.LogDebug("User not authenticated, returning empty permissions list");
                _cachedPermissions = Array.Empty<string>();
                _cacheExpiration = DateTime.UtcNow.Add(CacheDuration);
                _cachedUserName = null;
                return _cachedPermissions;
            }

            // Extract permissions from claims
            var permissions = user.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
                .AsReadOnly();

            _cachedPermissions = permissions;
            _cacheExpiration = DateTime.UtcNow.Add(CacheDuration);
            _cachedUserName = currentUserName;

            _logger.LogInformation("Loaded {Count} permissions for user {Username}", 
                permissions.Count, currentUserName ?? "unknown");

            return _cachedPermissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user permissions");
            return Array.Empty<string>();
        }
    }

    public async Task RefreshPermissionsAsync()
    {
        _logger.LogInformation("Refreshing permissions cache");
        _cachedPermissions = null;
        _cacheExpiration = DateTime.MinValue;
        _cachedUserName = null;
        await GetUserPermissionsAsync();
    }
}

