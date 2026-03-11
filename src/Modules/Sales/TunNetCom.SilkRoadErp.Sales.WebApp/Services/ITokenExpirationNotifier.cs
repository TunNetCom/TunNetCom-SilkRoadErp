namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public interface ITokenExpirationNotifier
{
    event Func<Task>? OnTokenExpired;
    Task NotifyTokenExpiredAsync();
}

