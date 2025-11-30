namespace TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

/// <summary>
/// Response DTO for print history information
/// </summary>
public record PrintHistoryResponse
{
    public long Id { get; init; }
    public string DocumentType { get; init; } = string.Empty;
    public int DocumentId { get; init; }
    public string PrintMode { get; init; } = string.Empty;
    public int? UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public DateTime PrintedAt { get; init; }
    public string? PrinterName { get; init; }
    public int Copies { get; init; } = 1;
    public string? FileName { get; init; }
    public bool IsDuplicata { get; init; }
}


