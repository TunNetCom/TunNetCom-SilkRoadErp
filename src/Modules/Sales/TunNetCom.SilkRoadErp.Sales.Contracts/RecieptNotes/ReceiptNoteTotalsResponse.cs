using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RecieptNotes;

public class ReceiptNoteTotalsResponse
{
    [JsonPropertyName("totalGrossAmount")]
    public decimal TotalGrossAmount { get; set; }

    [JsonPropertyName("totalVatAmount")]
    public decimal TotalVatAmount { get; set; }

    [JsonPropertyName("totalNetAmount")]
    public decimal TotalNetAmount { get; set; }
}
