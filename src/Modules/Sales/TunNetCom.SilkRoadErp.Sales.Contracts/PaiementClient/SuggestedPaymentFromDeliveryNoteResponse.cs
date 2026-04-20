namespace TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

public class SuggestedPaymentFromDeliveryNoteResponse
{
    public int DeliveryNoteId { get; set; }
    public int? InvoiceId { get; set; }
    public int? InvoiceNumber { get; set; }
    public decimal SuggestedAmount { get; set; }
}

