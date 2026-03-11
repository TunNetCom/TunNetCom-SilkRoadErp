namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class Plan
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MaxUsers { get; set; }
    public long MaxStorageBytes { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public int ApiRateLimitPerMinute { get; set; }
    public int TrialDays { get; set; }

    public virtual ICollection<PlanBoundedContext> PlanBoundedContexts { get; set; } = new List<PlanBoundedContext>();
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
