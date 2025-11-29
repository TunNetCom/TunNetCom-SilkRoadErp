namespace TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

/// <summary>
/// Response DTO for detailed audit log information with value comparison
/// </summary>
public record AuditLogDetailResponse
{
    public long Id { get; init; }
    public string EntityName { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public int? UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public Dictionary<string, object?>? OldValues { get; init; }
    public Dictionary<string, object?>? NewValues { get; init; }
    public List<string>? ChangedProperties { get; init; }
}


