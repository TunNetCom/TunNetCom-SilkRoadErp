namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class ReceiptNotesWithSummary
{
    [JsonPropertyName("totalGrossAmount")]
    public decimal TotalGrossAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }

    [JsonPropertyName("totalNetAmount")]
    public decimal TotalNetAmount { get; set; }

    [JsonPropertyName("ReceiptNotes")]
    public PagedList<ReceiptNoteDetailsResponse> ReceiptNotes { get; set; } = new PagedList<ReceiptNoteDetailsResponse>();
}
