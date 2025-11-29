namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class TokenExpirationNotifier : ITokenExpirationNotifier
{
    public event Func<Task>? OnTokenExpired;

    public async Task NotifyTokenExpiredAsync()
    {
        if (OnTokenExpired != null)
        {
            await OnTokenExpired.Invoke();
        }
    }
}

