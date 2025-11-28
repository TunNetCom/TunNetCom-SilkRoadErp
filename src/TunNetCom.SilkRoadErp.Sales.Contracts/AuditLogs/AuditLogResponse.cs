namespace TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

/// <summary>
/// Response DTO for basic audit log information
/// </summary>
public record AuditLogResponse
{
    public long Id { get; init; }
    public string EntityName { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public int? UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public List<string>? ChangedProperties { get; init; }
}

