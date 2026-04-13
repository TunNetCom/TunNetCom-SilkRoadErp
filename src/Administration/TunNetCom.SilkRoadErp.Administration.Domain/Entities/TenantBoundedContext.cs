namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class TenantBoundedContext
{
    public int Id { get; set; }
    public string TenantId { get; set; } = null!;
    public int BoundedContextId { get; set; }
    public bool IsEnabled { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
    public virtual BoundedContext BoundedContext { get; set; } = null!;
}
