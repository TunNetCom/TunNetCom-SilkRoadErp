namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(
    int NotificationId,
    int? UserId = null) : IRequest;

