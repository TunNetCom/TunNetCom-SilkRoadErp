namespace TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

/// <summary>
/// Request DTO for filtering print history
/// </summary>
public record GetPrintHistoryRequest
{
    public string? DocumentType { get; init; }
    public int? DocumentId { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public int? UserId { get; init; }
    public string? PrintMode { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}




