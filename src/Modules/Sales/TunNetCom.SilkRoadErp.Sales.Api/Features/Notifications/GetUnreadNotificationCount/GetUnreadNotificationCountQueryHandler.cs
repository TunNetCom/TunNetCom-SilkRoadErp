using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQueryHandler(
    SalesContext _context,
    ICurrentUserService _currentUserService,
    IMemoryCache _cache,
    ILogger<GetUnreadNotificationCountQueryHandler> _logger)
    : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    public async Task<int> Handle(
        GetUnreadNotificationCountQuery query,
        CancellationToken cancellationToken)
    {
        // IMPORTANT: never fall back to a global COUNT(*) when the user cannot be resolved.
        var userId = _currentUserService.GetUserId();
        if (!userId.HasValue || userId.Value <= 0)
        {
            _logger.LogWarning(
                "Unread notification count requested but current userId is missing/invalid. Returning 0.");
            return 0;
        }

        var cacheKey = $"notifications:unread-count:user:{userId.Value}";
        if (_cache.TryGetValue(cacheKey, out int cachedCount))
        {
            return cachedCount;
        }

        var countQuery = _context.Notification
            .AsNoTracking()
            .Where(n => !n.IsRead && n.UserId == userId.Value);

        var count = await countQuery.CountAsync(cancellationToken);

        // Short TTL: absorbs polling storms without making the badge feel stale.
        _cache.Set(cacheKey, count, TimeSpan.FromSeconds(10));

        _logger.LogInformation("Unread notification count: {Count} for user {UserId}", count, userId.Value);

        return count;
    }
}

