using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Notifications;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.GetNotifications;

public record GetNotificationsQuery(
    bool? UnreadOnly = null,
    int? UserId = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedList<NotificationResponse>>;

