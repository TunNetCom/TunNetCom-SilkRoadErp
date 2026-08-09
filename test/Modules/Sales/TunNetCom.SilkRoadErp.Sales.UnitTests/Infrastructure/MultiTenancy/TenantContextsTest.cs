using Microsoft.Extensions.Options;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.MultiTenancy;

public class TenantContextsTest
{
    [Fact]
    public void MultiTenantContext_Initially_ShouldNotBeResolved()
    {
        var context = new MultiTenantContext();

        context.IsResolved.Should().BeFalse();
        context.IsMultiTenant.Should().BeTrue();
        context.TenantId.Should().BeEmpty();
        context.CurrentTenant.Should().BeNull();
    }

    [Fact]
    public void MultiTenantContext_SetTenant_ShouldSetCurrentTenant()
    {
        var context = new MultiTenantContext();
        var tenant = new TenantInfo
        {
            Id = "tenant-1",
            Identifier = "tenant1",
            Name = "Tenant 1",
            Strategy = TenancyStrategy.SharedDatabaseSharedSchema,
            ConnectionString = "conn"
        };

        context.SetTenant(tenant);

        context.IsResolved.Should().BeTrue();
        context.TenantId.Should().Be("tenant-1");
        context.CurrentTenant.Should().BeSameAs(tenant);
    }

    [Fact]
    public void MultiTenantContext_SetTenant_Null_ShouldThrow()
    {
        var context = new MultiTenantContext();

        var act = () => context.SetTenant(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void StandaloneTenantContext_ShouldUseDefaultTenantId()
    {
        var options = Options.Create(new DeploymentOptions { DefaultTenantId = "standalone" });
        var context = new StandaloneTenantContext(options);

        context.TenantId.Should().Be("standalone");
        context.IsResolved.Should().BeTrue();
        context.IsMultiTenant.Should().BeFalse();
        context.CurrentTenant.Should().BeNull();
    }

    [Fact]
    public void StandaloneTenantContext_WhenDefaultNotConfigured_ShouldUseTenantConstants()
    {
        var options = Options.Create(new DeploymentOptions());
        var context = new StandaloneTenantContext(options);

        context.TenantId.Should().Be(TenantConstants.DefaultTenantId);
    }
}
