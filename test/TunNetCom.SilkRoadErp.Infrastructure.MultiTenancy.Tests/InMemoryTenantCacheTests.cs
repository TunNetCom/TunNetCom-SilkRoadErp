using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TunNetCom.SilkRoadErp.Infrastructure.Caching;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;
using TunNetCom.SilkRoadErp.SharedKernel.Caching;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;
using Xunit;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Tests;

public class InMemoryTenantCacheTests
{
    private InMemoryTenantCache CreateCache(string tenantId = "default")
    {
        var tenantContext = new StandaloneTenantContext(
            Options.Create(new DeploymentOptions { DefaultTenantId = tenantId }));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        return new InMemoryTenantCache(memoryCache, tenantContext);
    }

    [Fact]
    public async Task SetAndGet_ReturnsCachedValue()
    {
        var cache = CreateCache();
        await cache.SetAsync("key1", "value1");

        var result = await cache.GetAsync<string>("key1");
        result.Should().Be("value1");
    }

    [Fact]
    public async Task Get_ReturnsNull_WhenKeyNotFound()
    {
        var cache = CreateCache();

        var result = await cache.GetAsync<string>("nonexistent");
        result.Should().BeNull();
    }

    [Fact]
    public async Task Remove_RemovesCachedValue()
    {
        var cache = CreateCache();
        await cache.SetAsync("key1", "value1");
        await cache.RemoveAsync("key1");

        var result = await cache.GetAsync<string>("key1");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrSet_CallsFactory_WhenKeyNotFound()
    {
        var cache = CreateCache();

        var result = await cache.GetOrSetAsync("key1", _ => Task.FromResult("created"));
        result.Should().Be("created");

        var cached = await cache.GetAsync<string>("key1");
        cached.Should().Be("created");
    }

    [Fact]
    public async Task DifferentTenants_HaveIsolatedCaches()
    {
        var cache1 = CreateCache("tenant-a");
        var cache2 = CreateCache("tenant-b");

        await cache1.SetAsync("shared-key", "value-a");
        await cache2.SetAsync("shared-key", "value-b");

        var resultA = await cache1.GetAsync<string>("shared-key");
        var resultB = await cache2.GetAsync<string>("shared-key");

        resultA.Should().Be("value-a");
        resultB.Should().Be("value-b");
    }
}
