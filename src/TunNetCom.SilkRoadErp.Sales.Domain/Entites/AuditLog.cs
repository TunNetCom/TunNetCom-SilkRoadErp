#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public enum AuditAction
{
    Created = 1,
    Updated = 2,
    Deleted = 3
}

public class AuditLog
{
    private AuditLog()
    {
    }

    public static AuditLog Create(
        string entityName,
        string entityId,
        AuditAction action,
        int? userId,
        string? username,
        string? oldValues,
        string? newValues,
        string? changedProperties)
    {
        return new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            UserId = userId,
            Username = username ?? "System",
            Timestamp = DateTime.UtcNow,
            OldValues = oldValues,
            NewValues = newValues,
            ChangedProperties = changedProperties
        };
    }

    public void SetId(long id)
    {
        Id = id;
    }

    public long Id { get; private set; }

    public string EntityName { get; private set; } = null!;

    public string EntityId { get; private set; } = null!;

    public AuditAction Action { get; private set; }

    public int? UserId { get; private set; }

    public string Username { get; private set; } = null!;

    public DateTime Timestamp { get; private set; }

    public string? OldValues { get; private set; }

    public string? NewValues { get; private set; }

    public string? ChangedProperties { get; private set; }
}


