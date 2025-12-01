namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Middleware;

/// <summary>
/// Middleware to log authentication state after JWT Bearer authentication middleware runs
/// </summary>
public class AuthenticationDebugMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationDebugMiddleware> _logger;

    public AuthenticationDebugMiddleware(RequestDelegate next, ILogger<AuthenticationDebugMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only log for non-static file requests and non-health check
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (!path.Contains("/swagger") && 
            !path.Contains("/scalar") && 
            !path.Contains("/openapi") &&
            !path.Contains("/_framework") &&
            !path.Contains("/favicon") &&
            !path.Contains(".css") &&
            !path.Contains(".js") &&
            !path.Contains(".map"))
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            var hasAuthHeader = !string.IsNullOrEmpty(authHeader);
            var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;
            var userName = context.User?.Identity?.Name ?? "null";
            var claimsCount = context.User?.Claims.Count() ?? 0;
            
            _logger.LogInformation("AuthDebug - Request: {Method} {Path}, HasAuthHeader: {HasAuthHeader}, IsAuthenticated: {IsAuthenticated}, UserName: {UserName}, Claims: {ClaimsCount}",
                context.Request.Method, context.Request.Path, hasAuthHeader, isAuthenticated, userName, claimsCount);
            
            // Log all claims if authenticated
            if (isAuthenticated && context.User != null)
            {
                foreach (var claim in context.User.Claims)
                {
                    _logger.LogDebug("AuthDebug - User Claim: {Type} = {Value}", claim.Type, claim.Value);
                }
            }
            
            // If we have an auth header but user is not authenticated, something went wrong
            if (hasAuthHeader && !isAuthenticated)
            {
                _logger.LogWarning("AuthDebug - WARNING: Authorization header present but user NOT authenticated! Header: {Header}",
                    authHeader.Length > 50 ? authHeader.Substring(0, 50) + "..." : authHeader);
            }
        }

        await _next(context);
    }
}

