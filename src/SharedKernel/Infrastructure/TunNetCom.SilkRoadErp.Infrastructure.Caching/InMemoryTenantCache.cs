using Microsoft.Extensions.Caching.Memory;
using TunNetCom.SilkRoadErp.SharedKernel.Caching;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Infrastructure.Caching;

public sealed class InMemoryTenantCache : ITenantCache
{
    private readonly IMemoryCache _cache;
    private readonly ITenantContext _tenantContext;

    public InMemoryTenantCache(IMemoryCache cache, ITenantContext tenantContext)
    {
        _cache = cache;
        _tenantContext = tenantContext;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var prefixed = PrefixKey(key);
        _cache.TryGetValue(prefixed, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TenantCacheOptions? options = null, CancellationToken ct = default)
    {
        var prefixed = PrefixKey(key);
        var entry = _cache.CreateEntry(prefixed);
        entry.Value = value;

        if (options?.AbsoluteExpiration is { } abs)
            entry.AbsoluteExpirationRelativeToNow = abs;

        if (options?.SlidingExpiration is { } slide)
            entry.SlidingExpiration = slide;

        if (options is null)
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

        entry.Dispose(); // commits the entry
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _cache.Remove(PrefixKey(key));
        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TenantCacheOptions? options = null,
        CancellationToken ct = default)
    {
        var existing = await GetAsync<T>(key, ct);
        if (existing is not null)
            return existing;

        var value = await factory(ct);
        await SetAsync(key, value, options, ct);
        return value;
    }

    private string PrefixKey(string key) => $"tenant:{_tenantContext.TenantId}:{key}";
}
