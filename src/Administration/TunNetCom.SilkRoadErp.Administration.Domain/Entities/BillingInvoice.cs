namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class BillingInvoice
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public int Status { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }

    public virtual Subscription Subscription { get; set; } = null!;
}
