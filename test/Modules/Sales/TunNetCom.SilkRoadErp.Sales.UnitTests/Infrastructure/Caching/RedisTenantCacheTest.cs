using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TunNetCom.SilkRoadErp.Infrastructure.Caching;
using TunNetCom.SilkRoadErp.SharedKernel.Caching;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.Caching;

public class RedisTenantCacheTest
{
    private static Mock<IDistributedCache> CreateDistributedCache()
    {
        return new Mock<IDistributedCache>(MockBehavior.Strict);
    }

    private static Mock<ITenantContext> CreateContext(string tenantId = "tenant-1")
    {
        var ctx = new Mock<ITenantContext>();
        ctx.Setup(x => x.TenantId).Returns(tenantId);
        return ctx;
    }

    [Fact]
    public async Task GetAsync_WhenNoBytes_ShouldReturnDefault()
    {
        var distributed = CreateDistributedCache();
        distributed.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        var cache = new RedisTenantCache(distributed.Object, CreateContext().Object);

        var result = await cache.GetAsync<string>("key");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WhenEmptyBytes_ShouldReturnDefault()
    {
        var distributed = CreateDistributedCache();
        distributed.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<byte>());
        var cache = new RedisTenantCache(distributed.Object, CreateContext().Object);

        var result = await cache.GetAsync<string>("key");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldDeserializeJson()
    {
        var distributed = CreateDistributedCache();
        var payload = new TestPayload { Name = "John", Age = 30 };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        distributed.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);
        var cache = new RedisTenantCache(distributed.Object, CreateContext().Object);

        var result = await cache.GetAsync<TestPayload>("key");

        result.Should().NotBeNull();
        result!.Name.Should().Be("John");
        result.Age.Should().Be(30);
    }

    [Fact]
    public async Task SetAsync_ShouldStorePrefixedKeyAndOptions()
    {
        var distributed = CreateDistributedCache();
        string? capturedKey = null;
        DistributedCacheEntryOptions? capturedOptions = null;
        distributed.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (k, v, o, ct) => { capturedKey = k; capturedOptions = o; })
            .Returns(Task.CompletedTask);
        var cache = new RedisTenantCache(distributed.Object, CreateContext("tenant-1").Object);

        await cache.SetAsync("key", "value");

        capturedKey.Should().Be("tenant:tenant-1:key");
        capturedOptions.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public async Task SetAsync_WithAbsoluteExpiration_ShouldPassThrough()
    {
        var distributed = CreateDistributedCache();
        DistributedCacheEntryOptions? capturedOptions = null;
        distributed.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (k, v, o, ct) => capturedOptions = o)
            .Returns(Task.CompletedTask);
        var cache = new RedisTenantCache(distributed.Object, CreateContext().Object);

        await cache.SetAsync("key", "value", new TenantCacheOptions
        {
            AbsoluteExpiration = TimeSpan.FromMinutes(5)
        });

        capturedOptions!.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromMinutes(5));
    }

    [Fact]
    public async Task SetAsync_WithSlidingExpiration_ShouldPassThrough()
    {
        var distributed = CreateDistributedCache();
        DistributedCacheEntryOptions? capturedOptions = null;
        distributed.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (k, v, o, ct) => capturedOptions = o)
            .Returns(Task.CompletedTask);
        var cache = new RedisTenantCache(distributed.Object, CreateContext().Object);

        await cache.SetAsync("key", "value", new TenantCacheOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });

        capturedOptions!.SlidingExpiration.Should().Be(TimeSpan.FromMinutes(5));
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemovePrefixedKey()
    {
        var distributed = CreateDistributedCache();
        string? capturedKey = null;
        distributed.Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, CancellationToken>((k, ct) => capturedKey = k)
            .Returns(Task.CompletedTask);
        var cache = new RedisTenantCache(distributed.Object, CreateContext("tenant-9").Object);

        await cache.RemoveAsync("key");

        capturedKey.Should().Be("tenant:tenant-9:key");
    }

    [Fact]
    public async Task GetOrSetAsync_WhenMissing_ShouldCallFactoryAndCache()
    {
        var distributed = CreateDistributedCache();
        distributed.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        distributed.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var cache = new RedisTenantCache(distributed.Object, CreateContext().Object);
        var factoryCalls = 0;

        var result = await cache.GetOrSetAsync<string>("key", ct =>
        {
            factoryCalls++;
            return Task.FromResult("computed");
        });

        result.Should().Be("computed");
        factoryCalls.Should().Be(1);
    }

    [Fact]
    public async Task GetOrSetAsync_WhenCached_ShouldReturnCached()
    {
        var distributed = CreateDistributedCache();
        var bytes = JsonSerializer.SerializeToUtf8Bytes("cached", new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        distributed.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);
        var cache = new RedisTenantCache(distributed.Object, CreateContext().Object);
        var factoryCalls = 0;

        var result = await cache.GetOrSetAsync<string>("key", ct =>
        {
            factoryCalls++;
            return Task.FromResult("computed");
        });

        result.Should().Be("cached");
        factoryCalls.Should().Be(0);
    }

    private sealed class TestPayload
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
