namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Services;

public interface ICurrentUserService
{
    int? GetUserId();
    string? GetUsername();
    bool IsAuthenticated();
}


