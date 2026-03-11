namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public interface IAutoLogoutService
{
    Task HandleTokenExpirationAsync();
}

