namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class BoundedContext
{
    public int Id { get; set; }
    public string Key { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public bool IsCore { get; set; }
    public int DisplayOrder { get; set; }

    public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();
    public virtual ICollection<PlanBoundedContext> PlanBoundedContexts { get; set; } = new List<PlanBoundedContext>();
    public virtual ICollection<TenantBoundedContext> TenantBoundedContexts { get; set; } = new List<TenantBoundedContext>();
}
