namespace TunNetCom.SilkRoadErp.Sales.Contracts.Notifications;

public record GetNotificationsRequest(
    bool? UnreadOnly = null,
    int? UserId = null,
    int PageNumber = 1,
    int PageSize = 20);

