namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

public sealed class EntityNotFound : Error
{
    public EntityNotFound() : base()
    {
        Message = "not_found";
    }

    public static Error Error { get { return new EntityNotFound(); } }
}
 