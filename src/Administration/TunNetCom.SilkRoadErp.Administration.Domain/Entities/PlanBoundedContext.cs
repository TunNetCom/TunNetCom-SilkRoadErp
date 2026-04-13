namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class PlanBoundedContext
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public int BoundedContextId { get; set; }
    public bool IncludesAllFeatures { get; set; }

    public virtual Plan Plan { get; set; } = null!;
    public virtual BoundedContext BoundedContext { get; set; } = null!;
    public virtual ICollection<PlanFeature> PlanFeatures { get; set; } = new List<PlanFeature>();
}
