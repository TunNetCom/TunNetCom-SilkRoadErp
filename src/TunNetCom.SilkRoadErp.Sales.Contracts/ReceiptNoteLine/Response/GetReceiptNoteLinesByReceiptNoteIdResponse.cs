
using TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Classes;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Response;

public class GetReceiptNoteLinesByReceiptNoteIdResponse
{
    [JsonPropertyName("totalNetAmount")]
    public decimal TotalNetAmount { get; set; }

    [JsonPropertyName("totalGrossAmount")]
    public decimal TotalGrossAmount { get; set; }

    [JsonPropertyName("totalVatAmount")]
    public decimal TotalVatAmount { get; set; }

    [JsonPropertyName("ReceiptLinesBaseInfos")]
    public PagedList<ReceiptLine> ReceiptLinesBaseInfos { get; set; } = new PagedList<ReceiptLine>();
}
