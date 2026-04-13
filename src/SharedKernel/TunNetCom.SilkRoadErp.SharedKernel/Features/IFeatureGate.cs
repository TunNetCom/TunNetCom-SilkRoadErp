namespace TunNetCom.SilkRoadErp.SharedKernel.Features;

public interface IFeatureGate
{
    bool IsBoundedContextEnabled(string boundedContextKey);
    bool IsFeatureEnabled(string featureKey);
    IReadOnlySet<string> GetEnabledBoundedContexts();
    IReadOnlySet<string> GetEnabledFeatures();
    IReadOnlySet<string> GetPermissionsForFeature(string featureKey);
    string? GetFeatureForPermission(string permissionKey);
    bool IsMultiTenant { get; }
}
