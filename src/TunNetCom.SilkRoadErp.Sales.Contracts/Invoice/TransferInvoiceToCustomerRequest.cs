namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class TransferInvoiceToCustomerRequest
{
    [JsonPropertyName("targetCustomerId")]
    public int TargetCustomerId { get; set; }
}

