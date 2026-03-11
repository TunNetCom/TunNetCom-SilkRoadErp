namespace TunNetCom.SilkRoadErp.SharedKernel.Tenancy;

public sealed class DeploymentOptions
{
    public const string SectionName = "Deployment";
    public DeploymentMode Mode { get; set; } = DeploymentMode.Standalone;
    public string DefaultTenantId { get; set; } = TenantConstants.DefaultTenantId;
}
