using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Resolvers;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.MultiTenancy;

public class MultiTenancyServiceExtensionsTest
{
    private static IConfiguration CreateConfig(string mode)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Deployment:Mode"] = mode
            })
            .Build();
    }

    [Fact]
    public void AddMultiTenancy_WhenMultiTenant_ShouldRegisterResolversAndContexts()
    {
        var services = new ServiceCollection();
        var config = CreateConfig("MultiTenant");

        services.AddSingleton<IConfiguration>(config);
        services.AddLogging();
        services.AddMultiTenancy(config);
        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetService<MultiTenantContext>().Should().NotBeNull();
        scope.ServiceProvider.GetService<ITenantContext>().Should().BeOfType<MultiTenantContext>();
        scope.ServiceProvider.GetServices<ITenantResolver>().Should().HaveCount(3);
        scope.ServiceProvider.GetService<TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.EfCore.TenantSaveChangesInterceptor>()
            .Should().NotBeNull();
    }

    [Fact]
    public void AddMultiTenancy_WhenMultiTenant_ShouldConfigureDeploymentOptions()
    {
        var services = new ServiceCollection();
        var config = CreateConfig("MultiTenant");

        services.AddMultiTenancy(config);
        var provider = services.BuildServiceProvider();

        var options = provider.GetService<Microsoft.Extensions.Options.IOptions<DeploymentOptions>>();
        options.Should().NotBeNull();
    }

    [Fact]
    public void AddMultiTenancy_WhenStandalone_ShouldRegisterStandaloneContext()
    {
        var services = new ServiceCollection();
        var config = CreateConfig("Standalone");

        services.AddMultiTenancy(config);
        var provider = services.BuildServiceProvider();

        provider.GetService<ITenantContext>().Should().BeOfType<StandaloneTenantContext>();
        provider.GetService<TunNetCom.SilkRoadErp.SharedKernel.Features.IFeatureGate>()
            .Should().BeOfType<TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Features.StandaloneFeatureGate>();
        provider.GetServices<ITenantResolver>().Should().BeEmpty();
    }

    [Fact]
    public void AddMultiTenancy_WhenUnsetMode_ShouldDefaultToStandalone()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();

        services.AddMultiTenancy(config);
        var provider = services.BuildServiceProvider();

        provider.GetService<ITenantContext>().Should().BeOfType<StandaloneTenantContext>();
    }
}
