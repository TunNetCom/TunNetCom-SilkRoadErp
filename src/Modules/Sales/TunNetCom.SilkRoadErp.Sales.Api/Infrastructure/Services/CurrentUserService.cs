namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService, ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public int? GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("GetUserId: HttpContext is NULL");
            return null;
        }

        var user = httpContext.User;
        if (user == null)
        {
            _logger.LogWarning("GetUserId: HttpContext.User is NULL");
            return null;
        }

        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            _logger.LogWarning("GetUserId: NameIdentifier claim not found. Available claims: {Claims}", 
                string.Join(", ", user.Claims.Select(c => $"{c.Type}={c.Value}")));
            return null;
        }

        if (int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogDebug("GetUserId: Found userId={UserId}", userId);
            return userId;
        }

        _logger.LogWarning("GetUserId: Failed to parse userId from claim value '{Value}'", userIdClaim.Value);
        return null;
    }

    public string? GetUsername()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("GetUsername: HttpContext is NULL");
            return null;
        }

        var user = httpContext.User;
        if (user == null)
        {
            _logger.LogWarning("GetUsername: HttpContext.User is NULL");
            return null;
        }

        var usernameClaim = user.FindFirst(ClaimTypes.Name);
        if (usernameClaim == null)
        {
            _logger.LogWarning("GetUsername: Name claim not found. Available claims: {Claims}", 
                string.Join(", ", user.Claims.Select(c => $"{c.Type}={c.Value}")));
            return null;
        }

        _logger.LogDebug("GetUsername: Found username={Username}", usernameClaim.Value);
        return usernameClaim.Value;
    }

    public bool IsAuthenticated()
    {
        var isAuth = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        _logger.LogDebug("IsAuthenticated: {IsAuthenticated}", isAuth);
        return isAuth;
    }
}
