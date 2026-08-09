using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Infrastructure.Caching;
using TunNetCom.SilkRoadErp.SharedKernel.Caching;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.Caching;

public class InMemoryTenantCacheTest
{
    private static Mock<ITenantContext> CreateContext(string tenantId = "tenant-1")
    {
        var ctx = new Mock<ITenantContext>();
        ctx.Setup(x => x.TenantId).Returns(tenantId);
        return ctx;
    }

    private static InMemoryTenantCache CreateCache(Mock<ITenantContext> tenantContext)
    {
        return new InMemoryTenantCache(new MemoryCache(new MemoryCacheOptions()), tenantContext.Object);
    }

    [Fact]
    public async Task SetAsync_ThenGetAsync_ShouldRoundTripValue()
    {
        var cache = CreateCache(CreateContext("tenant-1"));

        await cache.SetAsync("key", "value");
        var result = await cache.GetAsync<string>("key");

        result.Should().Be("value");
    }

    [Fact]
    public async Task GetAsync_WhenMissing_ShouldReturnDefault()
    {
        var cache = CreateCache(CreateContext("tenant-1"));

        var result = await cache.GetAsync<string>("missing");

        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ShouldPrefixKeyPerTenant()
    {
        var cacheA = CreateCache(CreateContext("tenant-1"));
        var cacheB = CreateCache(CreateContext("tenant-2"));

        await cacheA.SetAsync("key", "value-a");
        await cacheB.SetAsync("key", "value-b");

        var resultA = await cacheA.GetAsync<string>("key");
        var resultB = await cacheB.GetAsync<string>("key");

        resultA.Should().Be("value-a");
        resultB.Should().Be("value-b");
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveEntry()
    {
        var cache = CreateCache(CreateContext("tenant-1"));
        await cache.SetAsync("key", "value");

        await cache.RemoveAsync("key");

        (await cache.GetAsync<string>("key")).Should().BeNull();
    }

    [Fact]
    public async Task GetOrSetAsync_WhenMissing_ShouldCallFactoryAndCache()
    {
        var cache = CreateCache(CreateContext("tenant-1"));
        var factoryCalls = 0;

        var result = await cache.GetOrSetAsync<string>("key", ct =>
        {
            factoryCalls++;
            return Task.FromResult("computed");
        });

        result.Should().Be("computed");
        factoryCalls.Should().Be(1);

        var cached = await cache.GetAsync<string>("key");
        cached.Should().Be("computed");
    }

    [Fact]
    public async Task GetOrSetAsync_WhenCached_ShouldNotCallFactory()
    {
        var cache = CreateCache(CreateContext("tenant-1"));
        await cache.SetAsync("key", "cached");
        var factoryCalls = 0;

        var result = await cache.GetOrSetAsync<string>("key", ct =>
        {
            factoryCalls++;
            return Task.FromResult("computed");
        });

        result.Should().Be("cached");
        factoryCalls.Should().Be(0);
    }

    [Fact]
    public async Task SetAsync_WithAbsoluteExpiration_ShouldRespectExpiration()
    {
        var cache = CreateCache(CreateContext("tenant-1"));

        await cache.SetAsync("key", "value", new TenantCacheOptions
        {
            AbsoluteExpiration = TimeSpan.FromMilliseconds(100)
        });
        await Task.Delay(200);

        (await cache.GetAsync<string>("key")).Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithSlidingExpiration_ShouldSetEntry()
    {
        var cache = CreateCache(CreateContext("tenant-1"));

        await cache.SetAsync("key", "value", new TenantCacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

        (await cache.GetAsync<string>("key")).Should().Be("value");
    }

    [Fact]
    public async Task SetAsync_WithNoOptions_ShouldDefaultTenMinutes()
    {
        var cache = CreateCache(CreateContext("tenant-1"));

        await cache.SetAsync("key", "value");

        (await cache.GetAsync<string>("key")).Should().Be("value");
    }
}
