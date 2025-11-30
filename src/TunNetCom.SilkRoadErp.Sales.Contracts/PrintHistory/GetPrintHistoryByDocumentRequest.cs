namespace TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

/// <summary>
/// Request DTO for getting print history by document
/// </summary>
public record GetPrintHistoryByDocumentRequest
{
    public string DocumentType { get; init; } = string.Empty;
    public int DocumentId { get; init; }
}