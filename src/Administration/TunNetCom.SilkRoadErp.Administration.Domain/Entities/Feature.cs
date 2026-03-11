namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class Feature
{
    public int Id { get; set; }
    public int BoundedContextId { get; set; }
    public string Key { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsCore { get; set; }

    public virtual BoundedContext BoundedContext { get; set; } = null!;
    public virtual ICollection<PlanFeature> PlanFeatures { get; set; } = new List<PlanFeature>();
    public virtual ICollection<TenantFeatureOverride> TenantFeatureOverrides { get; set; } = new List<TenantFeatureOverride>();
    public virtual ICollection<FeaturePermission> FeaturePermissions { get; set; } = new List<FeaturePermission>();
    public virtual ICollection<FeatureEndpointTag> FeatureEndpointTags { get; set; } = new List<FeatureEndpointTag>();
    public virtual ICollection<FeatureRoute> FeatureRoutes { get; set; } = new List<FeatureRoute>();
}
