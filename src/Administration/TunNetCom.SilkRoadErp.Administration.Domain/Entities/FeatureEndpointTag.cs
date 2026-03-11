namespace TunNetCom.SilkRoadErp.Administration.Domain.Entities;

public class FeatureEndpointTag
{
    public int Id { get; set; }
    public int FeatureId { get; set; }
    public string EndpointTag { get; set; } = null!;

    public virtual Feature Feature { get; set; } = null!;
}
