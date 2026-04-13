using Microsoft.EntityFrameworkCore;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.GetUnreadNotificationCount;

public class GetUnreadNotificationCountQueryHandler(
    SalesContext _context,
    ILogger<GetUnreadNotificationCountQueryHandler> _logger)
    : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    public async Task<int> Handle(
        GetUnreadNotificationCountQuery query,
        CancellationToken cancellationToken)
    {
        var countQuery = _context.Notification
            .AsNoTracking()
            .Where(n => !n.IsRead);

        // Filter by user (if specified, show global notifications (UserId == null) or user-specific)
        if (query.UserId.HasValue)
        {
            countQuery = countQuery.Where(n => n.UserId == null || n.UserId == query.UserId);
        }

        var count = await countQuery.CountAsync(cancellationToken);

        _logger.LogInformation("Unread notification count: {Count} for user {UserId}", count, query.UserId);

        return count;
    }
}

