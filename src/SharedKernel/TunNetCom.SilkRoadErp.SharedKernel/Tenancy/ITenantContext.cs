namespace TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

public interface ITenantContext
{
    string TenantId { get; }
    TenantInfo? CurrentTenant { get; }
    bool IsResolved { get; }
    bool IsMultiTenant { get; }
}
