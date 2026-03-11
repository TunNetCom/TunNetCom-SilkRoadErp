using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TunNetCom.SilkRoadErp.SharedKernel.Caching;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Infrastructure.Caching;

public sealed class RedisTenantCache : ITenantCache
{
    private readonly IDistributedCache _cache;
    private readonly ITenantContext _tenantContext;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RedisTenantCache(IDistributedCache cache, ITenantContext tenantContext)
    {
        _cache = cache;
        _tenantContext = tenantContext;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(PrefixKey(key), ct);
        if (bytes is null || bytes.Length == 0)
            return default;

        return JsonSerializer.Deserialize<T>(bytes, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TenantCacheOptions? options = null, CancellationToken ct = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
        var distOptions = new DistributedCacheEntryOptions();

        if (options?.AbsoluteExpiration is { } abs)
            distOptions.AbsoluteExpirationRelativeToNow = abs;

        if (options?.SlidingExpiration is { } slide)
            distOptions.SlidingExpiration = slide;

        if (options is null)
            distOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

        await _cache.SetAsync(PrefixKey(key), bytes, distOptions, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(PrefixKey(key), ct);
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
