using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler(
    NotificationService _notificationService,
    ILogger<MarkNotificationAsReadCommandHandler> _logger)
    : IRequestHandler<MarkNotificationAsReadCommand>
{
    public async Task Handle(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking notification {NotificationId} as read for user {UserId}",
            command.NotificationId, command.UserId);

        await _notificationService.MarkAsReadAsync(
            command.NotificationId,
            command.UserId,
            cancellationToken);
    }
}

