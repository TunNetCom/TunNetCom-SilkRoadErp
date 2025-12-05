using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Notifications;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public interface ISilkRoadNotificationService
{
    Task<PagedList<NotificationResponse>> GetNotificationsAsync(
        bool? unreadOnly = null,
        int? userId = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

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

