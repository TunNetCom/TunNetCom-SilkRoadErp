namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class UpdateProviderInvoiceDateRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}
