namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class CustomerAccount
{
    public int Id { get; set; }
    public string TenantId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "Owner";
    public bool IsOwner { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
}
