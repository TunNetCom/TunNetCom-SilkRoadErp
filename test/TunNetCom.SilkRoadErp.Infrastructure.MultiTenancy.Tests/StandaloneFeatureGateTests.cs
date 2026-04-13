using FluentAssertions;
using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Features;
using Xunit;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Tests;

public class StandaloneFeatureGateTests
{
    [Fact]
    public void IsMultiTenant_ReturnsFalse()
    {
        var gate = new StandaloneFeatureGate();
        gate.IsMultiTenant.Should().BeFalse();
    }

    [Fact]
    public void AllFeatures_AreEnabled()
    {
        var gate = new StandaloneFeatureGate();

        gate.IsBoundedContextEnabled("sales").Should().BeTrue();
        gate.IsFeatureEnabled("sales.invoicing").Should().BeTrue();
        gate.IsFeatureEnabled("anything").Should().BeTrue();
    }

    [Fact]
    public void GetFeatureForPermission_ReturnsNull()
    {
        var gate = new StandaloneFeatureGate();
        gate.GetFeatureForPermission("CanViewInvoices").Should().BeNull();
    }
}
