namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class FeatureRoute
{
    public int Id { get; set; }
    public int FeatureId { get; set; }
    public string RoutePattern { get; set; } = null!;

    public virtual Feature Feature { get; set; } = null!;
}
