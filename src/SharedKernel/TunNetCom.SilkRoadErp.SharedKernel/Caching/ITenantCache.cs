namespace TunNetCom.SilkRoadErp.SharedKernel.Caching;

public interface ITenantCache
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TenantCacheOptions? options = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task<T> GetOrSetAsync<T>(string key, Func<CancellationToken, Task<T>> factory,
        TenantCacheOptions? options = null, CancellationToken ct = default);
}
