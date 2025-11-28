using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

/// <summary>
/// Service pour vérifier les permissions de l'utilisateur
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IAuthService _authService;
    private readonly ILogger<PermissionService> _logger;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private IReadOnlyList<string>? _cachedPermissions;
    private DateTime _cacheExpiration = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public PermissionService(
        IAuthService authService,
        ILogger<PermissionService> logger,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _authService = authService;
        _logger = logger;
        _authenticationStateProvider = authenticationStateProvider;
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
        // Vérifier le cache
        if (_cachedPermissions != null && DateTime.UtcNow < _cacheExpiration)
        {
            _logger.LogDebug("Returning cached permissions. Count: {Count}", _cachedPermissions.Count);
            return _cachedPermissions;
        }

        try
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                _logger.LogDebug("User not authenticated, returning empty permissions list");
                _cachedPermissions = Array.Empty<string>();
                _cacheExpiration = DateTime.UtcNow.Add(CacheDuration);
                return _cachedPermissions;
            }

            // Extraire les permissions depuis les claims
            var permissions = user.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
                .AsReadOnly();

            _cachedPermissions = permissions;
            _cacheExpiration = DateTime.UtcNow.Add(CacheDuration);

            _logger.LogInformation("Loaded {Count} permissions for user {Username}", 
                permissions.Count, user.Identity?.Name ?? "unknown");

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
        await GetUserPermissionsAsync();
    }
}

