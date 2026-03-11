using FluentAssertions;
using Microsoft.Extensions.Options;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy;
using TunNetCom.SilkRoadErp.SharedKernel.Tenancy;
using Xunit;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Tests;

public class StandaloneTenantContextTests
{
    [Fact]
    public void TenantId_ReturnsDefaultTenantId()
    {
        var options = Options.Create(new DeploymentOptions
        {
            Mode = DeploymentMode.Standalone,
            DefaultTenantId = "default"
        });

        var context = new StandaloneTenantContext(options);

        context.TenantId.Should().Be("default");
        context.IsMultiTenant.Should().BeFalse();
        context.IsResolved.Should().BeTrue();
        context.CurrentTenant.Should().BeNull();
    }

    [Fact]
    public void TenantId_ReturnsCustomDefaultTenantId()
    {
        var options = Options.Create(new DeploymentOptions
        {
            Mode = DeploymentMode.Standalone,
            DefaultTenantId = "custom-default"
        });

        var context = new StandaloneTenantContext(options);

        context.TenantId.Should().Be("custom-default");
    }
}
