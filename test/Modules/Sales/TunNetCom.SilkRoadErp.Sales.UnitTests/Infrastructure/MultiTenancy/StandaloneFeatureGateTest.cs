using TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Features;

namespace TunNetCom.SilkRoadErp.Sales.UnitTests.Infrastructure.MultiTenancy;

public class StandaloneFeatureGateTest
{
    private readonly StandaloneFeatureGate _gate = new();

    [Fact]
    public void IsMultiTenant_ShouldBeFalse()
    {
        _gate.IsMultiTenant.Should().BeFalse();
    }

    [Fact]
    public void IsBoundedContextEnabled_ShouldReturnTrue()
    {
        _gate.IsBoundedContextEnabled("sales").Should().BeTrue();
    }

    [Fact]
    public void IsFeatureEnabled_ShouldReturnTrue()
    {
        _gate.IsFeatureEnabled("any").Should().BeTrue();
    }

    [Fact]
    public void GetEnabledBoundedContexts_ShouldReturnEmpty()
    {
        _gate.GetEnabledBoundedContexts().Should().BeEmpty();
    }

    [Fact]
    public void GetEnabledFeatures_ShouldReturnEmpty()
    {
        _gate.GetEnabledFeatures().Should().BeEmpty();
    }

    [Fact]
    public void GetPermissionsForFeature_ShouldReturnEmpty()
    {
        _gate.GetPermissionsForFeature("any").Should().BeEmpty();
    }

    [Fact]
    public void GetFeatureForPermission_ShouldReturnNull()
    {
        _gate.GetFeatureForPermission("any").Should().BeNull();
    }
}
