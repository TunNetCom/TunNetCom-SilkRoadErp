using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

public class NotificationData
{
    public NotificationType Type { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public int? UserId { get; set; }
}

