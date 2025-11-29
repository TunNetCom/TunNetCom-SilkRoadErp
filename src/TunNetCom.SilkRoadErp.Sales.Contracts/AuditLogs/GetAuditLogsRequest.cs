namespace TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

/// <summary>
/// Request DTO for filtering audit logs
/// </summary>
public record GetAuditLogsRequest
{
    public string? EntityName { get; init; }
    public string? EntityId { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public int? UserId { get; init; }
    public string? Action { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}


