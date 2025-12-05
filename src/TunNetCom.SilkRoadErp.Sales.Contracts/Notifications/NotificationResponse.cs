namespace TunNetCom.SilkRoadErp.Sales.Contracts.Notifications;

public record NotificationResponse
{
    public int Id { get; init; }
    public NotificationType Type { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public int? RelatedEntityId { get; init; }
    public string? RelatedEntityType { get; init; }
    public bool IsRead { get; init; }
    public int? UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ReadAt { get; init; }
}

