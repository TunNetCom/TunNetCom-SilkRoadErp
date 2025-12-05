namespace TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Notifications;

/// <summary>
/// Interface for sending notification emails.
/// This is a placeholder for future email notification functionality.
/// </summary>
public interface IEmailNotificationService
{
    /// <summary>
    /// Sends notification emails to configured recipients.
    /// </summary>
    /// <param name="notifications">The list of notifications to send via email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task SendNotificationEmailAsync(List<NotificationData> notifications, CancellationToken cancellationToken = default);
}

