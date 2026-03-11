using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

public class NotificationService
{
    private readonly SalesContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        SalesContext context,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateNotificationsAsync(List<NotificationData> notificationDataList, CancellationToken cancellationToken = default)
    {
        if (!notificationDataList.Any())
        {
            return;
        }

        var createdCount = 0;
        var skippedCount = 0;

        foreach (var data in notificationDataList)
        {
            try
            {
                // Check for duplicate notification (same type, same related entity, not read, created within last 24 hours)
                var duplicateExists = await _context.Notification
                    .AnyAsync(n =>
                        n.Type == data.Type &&
                        n.RelatedEntityId == data.RelatedEntityId &&
                        n.RelatedEntityType == data.RelatedEntityType &&
                        !n.IsRead &&
                        n.CreatedAt >= DateTime.UtcNow.AddHours(-24),
                        cancellationToken);

                if (duplicateExists)
                {
                    skippedCount++;
                    continue;
                }

                var notification = Notification.CreateNotification(
                    data.Type,
                    data.Title,
                    data.Message,
                    data.RelatedEntityId,
                    data.RelatedEntityType,
                    data.UserId);

                _context.Notification.Add(notification);
                createdCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification: {Title}", data.Title);
            }
        }

        if (createdCount > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created {Count} notifications, skipped {Skipped} duplicates", createdCount, skippedCount);
        }
    }

    public async Task MarkAsReadAsync(int notificationId, int? userId = null, CancellationToken cancellationToken = default)
    {
        var notification = await _context.Notification
            .FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);

        if (notification == null)
        {
            _logger.LogWarning("Notification {NotificationId} not found", notificationId);
            return;
        }

        // If userId is provided, only mark as read if it's a user-specific notification or global
        if (userId.HasValue && notification.UserId.HasValue && notification.UserId != userId)
        {
            _logger.LogWarning("User {UserId} cannot mark notification {NotificationId} as read", userId, notificationId);
            return;
        }

        notification.MarkAsRead();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(int? userId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Notification.Where(n => !n.IsRead);

        if (userId.HasValue)
        {
            query = query.Where(n => n.UserId == null || n.UserId == userId);
        }

        var notifications = await query.ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.MarkAsRead();
        }

        if (notifications.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Marked {Count} notifications as read for user {UserId}", notifications.Count, userId);
        }
    }
}

