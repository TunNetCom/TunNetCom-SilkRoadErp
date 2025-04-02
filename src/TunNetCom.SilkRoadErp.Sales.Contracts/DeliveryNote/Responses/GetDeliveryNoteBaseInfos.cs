namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

public class GetDeliveryNoteBaseInfos
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("numFacture")]
    public int? NumFacture { get; set; }

    [JsonPropertyName("netAmount")]
    public decimal NetAmount { get; set; }

    [JsonPropertyName("grossAmount")]
    public decimal GrossAmount { get; set; }

    [JsonPropertyName("vatAmount")]
    public decimal VatAmount { get; set; }
}
