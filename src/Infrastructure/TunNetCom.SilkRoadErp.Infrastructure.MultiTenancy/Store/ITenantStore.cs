using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Store;

public interface ITenantStore
{
    Task<TenantInfo?> GetByIdentifierAsync(string identifier, CancellationToken ct = default);
}
