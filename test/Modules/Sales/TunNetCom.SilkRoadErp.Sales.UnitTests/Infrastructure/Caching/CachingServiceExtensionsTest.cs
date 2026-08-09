using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TunNetCom.SilkRoadErp.Infrastructure.Caching;
using TunNetCom.SilkRoadErp.SharedKernel.Caching;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.Caching;

public class CachingServiceExtensionsTest
{
    private static void AddTenantContext(IServiceCollection services)
    {
        services.AddSingleton<ITenantContext>(Mock.Of<ITenantContext>(t => t.TenantId == "tenant-1"));
    }

    private static IConfiguration CreateConfig(string? provider)
    {
        var builder = new ConfigurationBuilder();
        if (provider != null)
        {
            builder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Caching:Provider"] = provider
            });
        }
        return builder.Build();
    }

    [Fact]
    public void AddTenantCaching_WhenProviderDefault_ShouldRegisterInMemory()
    {
        var services = new ServiceCollection();
        var config = CreateConfig(null);

        services.AddTenantCaching(config);
        AddTenantContext(services);
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetService<ITenantCache>().Should().BeOfType<InMemoryTenantCache>();
    }

    [Fact]
    public void AddTenantCaching_WhenProviderInMemory_ShouldRegisterInMemory()
    {
        var services = new ServiceCollection();
        var config = CreateConfig("InMemory");

        services.AddTenantCaching(config);
        AddTenantContext(services);
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetService<ITenantCache>().Should().BeOfType<InMemoryTenantCache>();
    }

    [Fact]
    public void AddTenantCaching_WhenProviderUnknown_ShouldRegisterInMemory()
    {
        var services = new ServiceCollection();
        var config = CreateConfig("Other");

        services.AddTenantCaching(config);
        AddTenantContext(services);
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetService<ITenantCache>().Should().BeOfType<InMemoryTenantCache>();
    }

    [Fact]
    public void AddTenantCaching_WhenProviderRedis_ShouldRegisterRedis()
    {
        var services = new ServiceCollection();
        var config = CreateConfig("Redis");

        services.AddTenantCaching(config);
        AddTenantContext(services);
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetService<ITenantCache>().Should().BeOfType<RedisTenantCache>();
    }
}
