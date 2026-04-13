using TunNetCom.SilkRoadErp.Administration.Domain.Enums;

namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class Subscription
{
    public int Id { get; set; }
    public string TenantId { get; set; } = null!;
    public int PlanId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string BillingCycle { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Plan Plan { get; set; } = null!;
    public virtual ICollection<BillingInvoice> BillingInvoices { get; set; } = new List<BillingInvoice>();
}
