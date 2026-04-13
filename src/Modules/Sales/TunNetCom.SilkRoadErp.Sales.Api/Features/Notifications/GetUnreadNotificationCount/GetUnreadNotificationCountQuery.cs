namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Notifications.GetUnreadNotificationCount;

public record GetUnreadNotificationCountQuery(
    int? UserId = null) : IRequest<int>;

