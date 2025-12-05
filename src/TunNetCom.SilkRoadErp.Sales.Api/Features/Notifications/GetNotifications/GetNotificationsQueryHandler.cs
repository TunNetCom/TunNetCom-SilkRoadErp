using Microsoft.EntityFrameworkCore;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Notifications;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.GetNotifications;

public class GetNotificationsQueryHandler(
    SalesContext _context,
    ILogger<GetNotificationsQueryHandler> _logger)
    : IRequestHandler<GetNotificationsQuery, PagedList<NotificationResponse>>
{
    public async Task<PagedList<NotificationResponse>> Handle(
        GetNotificationsQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetNotificationsQuery called with UnreadOnly={UnreadOnly}, UserId={UserId}, PageNumber={PageNumber}, PageSize={PageSize}",
            query.UnreadOnly, query.UserId, query.PageNumber, query.PageSize);

        var notificationsQuery = _context.Notification
            .AsNoTracking()
            .AsQueryable();

        // Filter by user (if specified, show global notifications (UserId == null) or user-specific)
        if (query.UserId.HasValue)
        {
            notificationsQuery = notificationsQuery.Where(n => n.UserId == null || n.UserId == query.UserId);
        }

        // Filter by read status
        if (query.UnreadOnly == true)
        {
            notificationsQuery = notificationsQuery.Where(n => !n.IsRead);
        }

        // Order by creation date (newest first)
        notificationsQuery = notificationsQuery.OrderByDescending(n => n.CreatedAt);

        // Project to response DTO
        var responseQuery = notificationsQuery.Select(n => new NotificationResponse
        {
            Id = n.Id,
            Type = (Contracts.Notifications.NotificationType)(int)n.Type,
            Title = n.Title,
            Message = n.Message,
            RelatedEntityId = n.RelatedEntityId,
            RelatedEntityType = n.RelatedEntityType,
            IsRead = n.IsRead,
            UserId = n.UserId,
            CreatedAt = n.CreatedAt,
            ReadAt = n.ReadAt
        });

        var pagedNotifications = await PagedList<NotificationResponse>.ToPagedListAsync(
            responseQuery,
            query.PageNumber,
            query.PageSize,
            cancellationToken);

        _logger.LogInformation("Retrieved {Count} notifications", pagedNotifications.Items.Count);

        return pagedNotifications;
    }
}

