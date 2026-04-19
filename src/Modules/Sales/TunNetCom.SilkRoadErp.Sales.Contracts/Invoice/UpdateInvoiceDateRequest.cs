namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class UpdateInvoiceDateRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}
