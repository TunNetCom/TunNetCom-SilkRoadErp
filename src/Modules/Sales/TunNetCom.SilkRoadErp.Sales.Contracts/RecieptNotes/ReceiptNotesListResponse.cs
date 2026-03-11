using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class ReceiptNotesListResponse
{
    [JsonPropertyName("receiptNotes")]
    public List<ReceiptNoteBaseInfo> ReceiptNotes { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totals")]
    public ReceiptNoteTotalsResponse Totals { get; set; } = new();
}
