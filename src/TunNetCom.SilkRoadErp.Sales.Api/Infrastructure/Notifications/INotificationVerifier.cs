namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

public interface INotificationVerifier
{
    Task<List<NotificationData>> VerifyAsync(CancellationToken cancellationToken);
}

