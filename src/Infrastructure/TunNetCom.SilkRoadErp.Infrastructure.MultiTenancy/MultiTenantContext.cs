using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;

public sealed class MultiTenantContext : ITenantContext
{
    public string TenantId => CurrentTenant?.Id ?? string.Empty;
    public TenantInfo? CurrentTenant { get; private set; }
    public bool IsResolved => CurrentTenant is not null;
    public bool IsMultiTenant => true;

    public void SetTenant(TenantInfo tenant)
    {
        CurrentTenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
    }
}
