namespace TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

/// <summary>
/// Request DTO for creating a print history record
/// </summary>
public record CreatePrintHistoryRequest
{
    public string DocumentType { get; init; } = string.Empty;
    public int DocumentId { get; init; }
    public string PrintMode { get; init; } = string.Empty;
    public int? UserId { get; init; }
    public string? Username { get; init; }
    public string? PrinterName { get; init; }
    public int Copies { get; init; } = 1;
    public string? FileName { get; init; }
    public bool IsDuplicata { get; init; } = false;
}

