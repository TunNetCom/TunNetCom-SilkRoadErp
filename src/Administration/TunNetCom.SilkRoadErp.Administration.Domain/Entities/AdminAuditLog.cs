namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class AdminAuditLog
{
    public long Id { get; set; }
    public string Action { get; set; } = null!;
    public string TargetType { get; set; } = null!;
    public string TargetId { get; set; } = null!;
    public string PerformedBy { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Reason { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
