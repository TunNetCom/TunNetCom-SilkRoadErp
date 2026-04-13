namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class TenantFeatureOverride
{
    public int Id { get; set; }
    public string TenantId { get; set; } = null!;
    public int FeatureId { get; set; }
    public bool IsEnabled { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Feature Feature { get; set; } = null!;
}
