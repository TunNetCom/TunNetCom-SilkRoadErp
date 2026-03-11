using TunNetCom.SilkRoadErp.SharedKernel.Features;

namespace TunNetCom.SilkRoadErp.Infrastructure.MultiTenancy.Features;

public sealed class StandaloneFeatureGate : IFeatureGate
{
    private static readonly IReadOnlySet<string> EmptySet = new HashSet<string>();

    public bool IsMultiTenant => false;
    public bool IsBoundedContextEnabled(string boundedContextKey) => true;
    public bool IsFeatureEnabled(string featureKey) => true;
    public IReadOnlySet<string> GetEnabledBoundedContexts() => EmptySet;
    public IReadOnlySet<string> GetEnabledFeatures() => EmptySet;
    public IReadOnlySet<string> GetPermissionsForFeature(string featureKey) => EmptySet;
    public string? GetFeatureForPermission(string permissionKey) => null;
}
