using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommandHandler(
    NotificationService _notificationService,
    ILogger<MarkAllNotificationsAsReadCommandHandler> _logger)
    : IRequestHandler<MarkAllNotificationsAsReadCommand>
{
    public async Task Handle(
        MarkAllNotificationsAsReadCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking all notifications as read for user {UserId}", command.UserId);

        await _notificationService.MarkAllAsReadAsync(
            command.UserId,
            cancellationToken);
    }
}

