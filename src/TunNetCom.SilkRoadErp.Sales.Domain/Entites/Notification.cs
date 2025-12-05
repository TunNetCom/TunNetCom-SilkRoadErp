#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class Notification
{
    private Notification()
    {
    }

    public static Notification CreateNotification(
        NotificationType type,
        string title,
        string message,
        int? relatedEntityId = null,
        string? relatedEntityType = null,
        int? userId = null)
    {
        return new Notification
        {
            Type = type,
            Title = title,
            Message = message,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
            UserId = userId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }

    public int Id { get; private set; }

    public NotificationType Type { get; private set; }

    public string Title { get; private set; } = null!;

    public string Message { get; private set; } = null!;

    public int? RelatedEntityId { get; private set; }

    public string? RelatedEntityType { get; private set; }

    public bool IsRead { get; private set; }

    public int? UserId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ReadAt { get; private set; }

    public virtual User? User { get; set; }
}

