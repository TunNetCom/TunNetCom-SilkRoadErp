using Microsoft.Extensions.Options;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;

public sealed class StandaloneTenantContext : ITenantContext
{
    public StandaloneTenantContext(IOptions<DeploymentOptions> options)
    {
        TenantId = options.Value.DefaultTenantId;
    }

    public string TenantId { get; }
    public TenantInfo? CurrentTenant => null;
    public bool IsResolved => true;
    public bool IsMultiTenant => false;
}
