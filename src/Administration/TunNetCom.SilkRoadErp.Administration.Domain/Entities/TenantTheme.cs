namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class TenantTheme
{
    public int Id { get; set; }
    public string TenantId { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public string? FaviconUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? AccentColor { get; set; }
    public string? CompanyDisplayName { get; set; }
    public string? CustomCss { get; set; }
    public string? CustomDomain { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
}
