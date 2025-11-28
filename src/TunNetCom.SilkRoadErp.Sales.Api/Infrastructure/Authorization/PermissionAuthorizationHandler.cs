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
        _logger.LogInformation("PermissionAuthorizationHandler: Checking permission '{Permission}'", requirement.Permission);
        
        // Log all user information for debugging
        _logger.LogInformation("PermissionAuthorizationHandler: User Identity Name: {Name}, AuthenticationType: {AuthType}", 
            context.User.Identity?.Name ?? "null", context.User.Identity?.AuthenticationType ?? "null");
        _logger.LogInformation("PermissionAuthorizationHandler: Claims count: {Count}", context.User.Claims.Count());
        foreach (var claim in context.User.Claims)
        {
            _logger.LogInformation("PermissionAuthorizationHandler: Claim - {Type} = {Value}", claim.Type, claim.Value);
        }
        
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        _logger.LogInformation("PermissionAuthorizationHandler: User authenticated: {IsAuthenticated}", isAuthenticated);
        
        if (!isAuthenticated)
        {
            _logger.LogWarning("PermissionAuthorizationHandler: User is not authenticated. Identity: {Identity}, Claims count: {ClaimsCount}", 
                context.User.Identity?.Name ?? "null", context.User.Claims.Count());
            return;
        }

        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        _logger.LogInformation("PermissionAuthorizationHandler: UserId claim: {UserIdClaim}", userIdClaim?.Value ?? "null");
        
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("PermissionAuthorizationHandler: Invalid or missing UserId claim");
            return;
        }

        _logger.LogInformation("PermissionAuthorizationHandler: Checking permission for user {UserId}", userId);

        // Check if user has the required permission through their roles
        var hasPermission = await _context.User
            .Where(u => u.Id == userId && u.IsActive)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Name == requirement.Permission);

        _logger.LogInformation("PermissionAuthorizationHandler: User {UserId} has permission '{Permission}': {HasPermission}", 
            userId, requirement.Permission, hasPermission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}

