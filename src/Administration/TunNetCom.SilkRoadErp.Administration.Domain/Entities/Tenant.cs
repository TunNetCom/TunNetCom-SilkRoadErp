using TunNetCom.SilkRoadErp.Administration.Domain.Enums;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class Tenant
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Identifier { get; set; } = null!;
    public string Name { get; set; } = null!;
    public TenancyStrategy Strategy { get; set; } = TenancyStrategy.SharedDatabaseSharedSchema;
    public string ConnectionString { get; set; } = null!;
    public string? SchemaName { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.Provisioning;
    public string? BlockReason { get; set; }
    public int GracePeriodDays { get; set; } = 15;
    public DateTime? BlockedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public virtual ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
    public virtual ICollection<TenantBoundedContext> TenantBoundedContexts { get; set; } = new List<TenantBoundedContext>();
    public virtual ICollection<TenantFeatureOverride> TenantFeatureOverrides { get; set; } = new List<TenantFeatureOverride>();
}
