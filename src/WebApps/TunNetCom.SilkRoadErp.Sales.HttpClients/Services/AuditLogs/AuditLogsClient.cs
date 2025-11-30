using System.Web;
using TunNetCom.SilkRoadErp.Sales.Contracts.AuditLogs;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AuditLogs;

public class AuditLogsClient : IAuditLogsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditLogsClient> _logger;

    public AuditLogsClient(HttpClient httpClient, ILogger<AuditLogsClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedList<AuditLogResponse>> GetAuditLogsAsync(
        GetAuditLogsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Build query string
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

            if (!string.IsNullOrEmpty(request.EntityName))
            {
                queryString += $"&entityName={Uri.EscapeDataString(request.EntityName)}";
            }

            if (!string.IsNullOrEmpty(request.EntityId))
            {
                queryString += $"&entityId={Uri.EscapeDataString(request.EntityId)}";
            }

            if (request.DateFrom.HasValue)
            {
                queryString += $"&dateFrom={Uri.EscapeDataString(request.DateFrom.Value.ToString("o"))}";
            }

            if (request.DateTo.HasValue)
            {
                queryString += $"&dateTo={Uri.EscapeDataString(request.DateTo.Value.ToString("o"))}";
            }

            if (request.UserId.HasValue)
            {
                queryString += $"&userId={request.UserId.Value}";
            }

            if (!string.IsNullOrEmpty(request.Action))
            {
                queryString += $"&action={Uri.EscapeDataString(request.Action)}";
            }

            var response = await _httpClient.GetAsync($"/audit-logs{queryString}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedList<AuditLogResponse>>(cancellationToken: cancellationToken);
                return result ?? new PagedList<AuditLogResponse>();
            }

            throw new Exception($"Failed to fetch audit logs. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching audit logs");
            throw;
        }
    }

    public async Task<List<AuditLogDetailResponse>> GetAuditLogsByEntityAsync(
        string entityName,
        string entityId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/audit-logs/{Uri.EscapeDataString(entityName)}/{Uri.EscapeDataString(entityId)}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<AuditLogDetailResponse>>(cancellationToken: cancellationToken);
                return result ?? new List<AuditLogDetailResponse>();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new List<AuditLogDetailResponse>();
            }

            throw new Exception($"Failed to fetch audit logs for entity. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching audit logs for entity {EntityName}:{EntityId}", entityName, entityId);
            throw;
        }
    }
}