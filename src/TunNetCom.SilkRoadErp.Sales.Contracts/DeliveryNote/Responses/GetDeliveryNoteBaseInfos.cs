namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

public class GetDeliveryNoteBaseInfos
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

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

    [JsonPropertyName("customerId")]
    public int? CustomerId { get; set; }

    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; } = "SOPAL";
}
