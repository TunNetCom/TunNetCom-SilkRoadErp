using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Notifications;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Notifications;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class SilkRoadNotificationService : ISilkRoadNotificationService
{
    private readonly INotificationApiClient _apiClient;
    private readonly ILogger<SilkRoadNotificationService> _logger;

    public SilkRoadNotificationService(
        INotificationApiClient apiClient,
        ILogger<SilkRoadNotificationService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<PagedList<NotificationResponse>> GetNotificationsAsync(
        bool? unreadOnly = null,
        int? userId = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetNotificationsRequest(unreadOnly, userId, pageNumber, pageSize);
            return await _apiClient.GetNotificationsAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications");
            throw;
        }
    }

    public async Task<int> GetUnreadNotificationCountAsync(
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _apiClient.GetUnreadNotificationCountAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread notification count");
            return 0; // Return 0 on error to avoid breaking the UI
        }
    }

    public async Task MarkNotificationAsReadAsync(
        int notificationId,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _apiClient.MarkNotificationAsReadAsync(notificationId, userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read");
            throw;
        }
    }

    public async Task MarkAllNotificationsAsReadAsync(
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _apiClient.MarkAllNotificationsAsReadAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            throw;
        }
    }
}

