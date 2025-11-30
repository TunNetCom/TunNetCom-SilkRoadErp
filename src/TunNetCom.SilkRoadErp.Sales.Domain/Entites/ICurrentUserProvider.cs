namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

/// <summary>
/// Interface for getting the current user information for audit logging.
/// This interface is implemented in the API layer to avoid circular dependencies.
/// </summary>
public interface ICurrentUserProvider
{
    int? GetUserId();
    string? GetUsername();
    bool IsAuthenticated();
}






