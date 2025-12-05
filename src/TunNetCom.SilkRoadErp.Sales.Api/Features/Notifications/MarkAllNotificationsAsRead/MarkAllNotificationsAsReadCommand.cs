namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.MarkAllNotificationsAsRead;

public record MarkAllNotificationsAsReadCommand(
    int? UserId = null) : IRequest;

