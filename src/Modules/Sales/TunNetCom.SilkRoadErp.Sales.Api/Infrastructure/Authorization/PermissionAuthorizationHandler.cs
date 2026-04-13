using TunNetCom.SilkRoadErp.SharedKernel.Features;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly SalesContext _context;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;
    private readonly IFeatureGate _featureGate;

    public PermissionAuthorizationHandler(
        SalesContext context,
        ILogger<PermissionAuthorizationHandler> logger,
        IFeatureGate featureGate)
    {
        _context = context;
        _logger = logger;
        _featureGate = featureGate;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            _logger.LogWarning("Permission check failed: User not authenticated for '{Permission}'", requirement.Permission);
            return;
        }

        if (_featureGate.IsMultiTenant)
        {
            var featureKey = _featureGate.GetFeatureForPermission(requirement.Permission);
            if (featureKey is not null && !_featureGate.IsFeatureEnabled(featureKey))
            {
                _logger.LogWarning("Feature '{Feature}' not enabled for tenant, permission '{Permission}' blocked",
                    featureKey, requirement.Permission);
                return;
            }
        }

        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("Permission check failed: Invalid UserId claim for '{Permission}'", requirement.Permission);
            return;
        }

        var hasPermission = await _context.User
            .Where(u => u.Id == userId && u.IsActive)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Name == requirement.Permission);

        if (hasPermission)
        {
            _logger.LogDebug("Permission '{Permission}' granted to user {UserId}", requirement.Permission, userId);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("Permission '{Permission}' denied to user {UserId} ({Username})", 
                requirement.Permission, userId, context.User.Identity?.Name);
        }
    }
}

