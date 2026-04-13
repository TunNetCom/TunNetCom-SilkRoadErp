using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Notifications;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Notifications;

public interface INotificationApiClient
{
    Task<PagedList<NotificationResponse>> GetNotificationsAsync(
        GetNotificationsRequest request,
        CancellationToken cancellationToken);

    Task<int> GetUnreadNotificationCountAsync(
        int? userId = null,
        CancellationToken cancellationToken = default);

    Task MarkNotificationAsReadAsync(
        int notificationId,
        int? userId = null,
        CancellationToken cancellationToken = default);

    Task MarkAllNotificationsAsReadAsync(
        int? userId = null,
        CancellationToken cancellationToken = default);
}

