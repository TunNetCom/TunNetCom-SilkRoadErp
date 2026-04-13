namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class PlanFeature
{
    public int Id { get; set; }
    public int PlanBoundedContextId { get; set; }
    public int FeatureId { get; set; }

    public virtual PlanBoundedContext PlanBoundedContext { get; set; } = null!;
    public virtual Feature Feature { get; set; } = null!;
}
