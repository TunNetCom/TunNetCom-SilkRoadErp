using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly SalesContext _context;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;

    public PermissionAuthorizationHandler(SalesContext context, ILogger<PermissionAuthorizationHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Check if user is authenticated
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            _logger.LogWarning("Permission check failed: User not authenticated for '{Permission}'", requirement.Permission);
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("Permission check failed: Invalid UserId claim for '{Permission}'", requirement.Permission);
            return;
        }

        // Check if user has the required permission through their roles
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

