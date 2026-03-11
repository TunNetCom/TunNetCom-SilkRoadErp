namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class FeaturePermission
{
    public int Id { get; set; }
    public int FeatureId { get; set; }
    public string PermissionKey { get; set; } = null!;

    public virtual Feature Feature { get; set; } = null!;
}
