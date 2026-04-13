namespace TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

/// <summary>
/// Default values for tenant-related properties. Use this constant instead of hardcoding the literal
/// so the default tenant id is defined in one place.
/// </summary>
public static class TenantConstants
{
    /// <summary>
    /// Default tenant identifier used when no tenant context is set (e.g. standalone mode).
    /// </summary>
    public const string DefaultTenantId = "default";
}
