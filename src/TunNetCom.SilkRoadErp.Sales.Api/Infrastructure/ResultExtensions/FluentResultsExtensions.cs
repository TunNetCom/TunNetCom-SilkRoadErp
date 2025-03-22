namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.ResultExtensions;

public sealed class EntityNotFound : Error
{
    private EntityNotFound(string message) : base()
    {
        Message = message;
    }

    public static EntityNotFound Error(string message = "not_found") => new EntityNotFound(message);
}
 