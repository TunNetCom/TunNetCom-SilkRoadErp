using System.Web;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Notifications;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Notifications;

public class NotificationApiClient : INotificationApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationApiClient> _logger;

    public NotificationApiClient(HttpClient httpClient, ILogger<NotificationApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedList<NotificationResponse>> GetNotificationsAsync(
        GetNotificationsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

            if (request.UnreadOnly.HasValue)
            {
                queryString += $"&unreadOnly={request.UnreadOnly.Value}";
            }

            if (request.UserId.HasValue)
            {
                queryString += $"&userId={request.UserId.Value}";
            }

            var response = await _httpClient.GetAsync($"/notifications{queryString}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedList<NotificationResponse>>(cancellationToken: cancellationToken);
                return result ?? new PagedList<NotificationResponse>();
            }

            throw new Exception($"Failed to fetch notifications. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching notifications");
            throw;
        }
    }

    public async Task<int> GetUnreadNotificationCountAsync(
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryString = userId.HasValue ? $"?userId={userId.Value}" : string.Empty;
            var response = await _httpClient.GetAsync($"/notifications/unread-count{queryString}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UnreadCountResponse>(cancellationToken: cancellationToken);
                return result?.count ?? 0;
            }

            throw new Exception($"Failed to fetch unread notification count. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching unread notification count");
            throw;
        }
    }

    public async Task MarkNotificationAsReadAsync(
        int notificationId,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryString = userId.HasValue ? $"?userId={userId.Value}" : string.Empty;
            var response = await _httpClient.PutAsync($"/notifications/{notificationId}/mark-read{queryString}", null, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to mark notification as read. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
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
            var queryString = userId.HasValue ? $"?userId={userId.Value}" : string.Empty;
            var response = await _httpClient.PutAsync($"/notifications/mark-all-read{queryString}", null, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to mark all notifications as read. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            throw;
        }
    }

    private record UnreadCountResponse(int count);
}

