using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public class ODataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ODataService> _logger;

    public ODataService(HttpClient httpClient, ILogger<ODataService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ODataResponse<T>> QueryAsync<T>(
        string entitySet,
        LoadDataArgs? args = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? customerId = null,
        int? providerId = null,
        int? technicianId = null,
        List<int>? tagIds = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ODataService.QueryAsync called for entitySet: {EntitySet}", entitySet);
        try
        {
            // Build base URL with entity set
            var queryBuilder = new StringBuilder($"/odata/{entitySet}");
            
            // Add query parameters
            var queryParams = new List<string> { "$count=true" };

            // Build $filter clause
            var filters = new List<string>();

            // Date and customer filters are applied server-side via query parameters
            // Add them as query parameters instead of OData $filter
            if (startDate.HasValue)
            {
                var startDateStr = startDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                queryParams.Add($"startDate={Uri.EscapeDataString(startDateStr)}");
            }

            if (endDate.HasValue)
            {
                var endDateStr = endDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                queryParams.Add($"endDate={Uri.EscapeDataString(endDateStr)}");
            }

            if (customerId.HasValue)
            {
                queryParams.Add($"customerId={customerId.Value}");
            }

            if (providerId.HasValue)
            {
                queryParams.Add($"providerId={providerId.Value}");
            }

            if (technicianId.HasValue)
            {
                queryParams.Add($"technicianId={technicianId.Value}");
            }

            if (tagIds != null && tagIds.Any())
            {
                foreach (var tagId in tagIds)
                {
                    queryParams.Add($"tagIds={tagId}");
                }
            }

            // Add Radzen filter if provided
            if (!string.IsNullOrEmpty(args?.Filter?.ToString()))
            {
                // Convert Radzen filter to OData filter
                var radzenFilter = args.Filter.ToString();
                // Simple conversion - may need more sophisticated parsing
                if (radzenFilter.Contains("contains", StringComparison.OrdinalIgnoreCase))
                {
                    // Handle contains filter
                    var match = System.Text.RegularExpressions.Regex.Match(radzenFilter, @"contains\((\w+),\s*'([^']+)'\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var property = match.Groups[1].Value;
                        var value = match.Groups[2].Value;
                        filters.Add($"contains({property}, '{value}')");
                    }
                }
                else
                {
                    // Try to parse as simple property filter
                    filters.Add(radzenFilter);
                }
            }

            if (filters.Any())
            {
                var filterString = string.Join(" and ", filters.Select(f => $"({f})"));
                queryParams.Add($"$filter={Uri.EscapeDataString(filterString)}");
            }

            // Add $orderby
            if (!string.IsNullOrEmpty(args?.OrderBy?.ToString()))
            {
                var orderBy = args.OrderBy.ToString();
                // Convert Radzen orderby format to OData (e.g., "Date desc" -> "Date desc")
                queryParams.Add($"$orderby={Uri.EscapeDataString(orderBy)}");
            }

            // Add $top and $skip for pagination
            if (args?.Top.HasValue == true)
            {
                queryParams.Add($"$top={args.Top.Value}");
            }

            if (args?.Skip.HasValue == true)
            {
                queryParams.Add($"$skip={args.Skip.Value}");
            }

            // Build final URL
            var url = queryBuilder.ToString();
            if (queryParams.Any())
            {
                url += "?" + string.Join("&", queryParams);
            }
            _logger.LogInformation("OData query: {Url}", url);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("OData request failed with status {StatusCode}. Response: {ErrorContent}", 
                    response.StatusCode, errorContent);
                throw new HttpRequestException($"OData request failed with status {response.StatusCode}: {errorContent}");
            }
            
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var odataResponse = JsonSerializer.Deserialize<ODataResponse<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return odataResponse ?? new ODataResponse<T> { Value = new List<T>() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying OData endpoint {EntitySet}", entitySet);
            return new ODataResponse<T> { Value = new List<T>() };
        }
    }
}

public class ODataResponse<T>
{
    [JsonPropertyName("@odata.context")]
    public string? ODataContext { get; set; }

    [JsonPropertyName("@odata.count")]
    public int? ODataCount { get; set; }

    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = new();
}

