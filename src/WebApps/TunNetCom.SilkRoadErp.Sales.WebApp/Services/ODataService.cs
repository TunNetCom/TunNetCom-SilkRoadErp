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

            // Add Radzen filter if provided - OData will handle it automatically via [EnableQuery]
            // Just pass it through as $filter parameter
            if (!string.IsNullOrEmpty(args?.Filter?.ToString()))
            {
                var radzenFilter = args.Filter.ToString()!;
                _logger.LogInformation("Processing Radzen filter: {Filter}", radzenFilter);
                
                // Check if filter is already in OData format
                // Remove leading/trailing parentheses and whitespace for detection
                var trimmedFilter = radzenFilter.Trim();
                while (trimmedFilter.StartsWith("(") && trimmedFilter.EndsWith(")"))
                {
                    trimmedFilter = trimmedFilter.Substring(1, trimmedFilter.Length - 2).Trim();
                }
                
                // Check if it's OData format: contains function calls, comparison operators, or logical operators with OData functions
                bool isODataFormat = trimmedFilter.StartsWith("contains(", StringComparison.OrdinalIgnoreCase) ||
                                    trimmedFilter.StartsWith("startswith(", StringComparison.OrdinalIgnoreCase) ||
                                    trimmedFilter.StartsWith("endswith(", StringComparison.OrdinalIgnoreCase) ||
                                    radzenFilter.Contains(" eq ", StringComparison.OrdinalIgnoreCase) ||
                                    radzenFilter.Contains(" ne ", StringComparison.OrdinalIgnoreCase) ||
                                    radzenFilter.Contains(" gt ", StringComparison.OrdinalIgnoreCase) ||
                                    radzenFilter.Contains(" ge ", StringComparison.OrdinalIgnoreCase) ||
                                    radzenFilter.Contains(" lt ", StringComparison.OrdinalIgnoreCase) ||
                                    radzenFilter.Contains(" le ", StringComparison.OrdinalIgnoreCase) ||
                                    (radzenFilter.Contains(" and ", StringComparison.OrdinalIgnoreCase) && radzenFilter.Contains("contains(", StringComparison.OrdinalIgnoreCase)) ||
                                    (radzenFilter.Contains(" or ", StringComparison.OrdinalIgnoreCase) && radzenFilter.Contains("contains(", StringComparison.OrdinalIgnoreCase));
                
                if (isODataFormat)
                {
                    // Filter is already in OData format, use it directly
                    filters.Add(radzenFilter);
                    _logger.LogInformation("Using filter as OData format: {Filter}", radzenFilter);
                }
                else
                {
                    // Try to convert Radzen format to OData
                    // Radzen FilterMode.Simple often sends: "Property contains 'value'" or "Property='value'"
                    var radzenContainsMatch = System.Text.RegularExpressions.Regex.Match(
                        radzenFilter, 
                        @"(\w+)\s+contains\s+['""]([^'""]+)['""]", 
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                    if (radzenContainsMatch.Success)
                    {
                        var property = radzenContainsMatch.Groups[1].Value;
                        var value = radzenContainsMatch.Groups[2].Value;
                        var odataFilter = $"contains({property}, '{value}')";
                        filters.Add(odataFilter);
                        _logger.LogInformation("Converted Radzen contains format to OData: {ODataFilter}", odataFilter);
                    }
                    else if (radzenFilter.Contains('=') && !radzenFilter.Contains(" eq ") && !radzenFilter.Contains(" ne "))
                    {
                        // Property=value format
                        var parts = radzenFilter.Split('=', 2);
                        if (parts.Length == 2)
                        {
                            var property = parts[0].Trim();
                            var value = parts[1].Trim().Trim('\'', '"');
                            filters.Add($"contains({property}, '{value}')");
                            _logger.LogInformation("Converted property=value filter to OData: contains({Property}, '{Value}')", property, value);
                        }
                        else
                        {
                            filters.Add(radzenFilter);
                        }
                    }
                    else
                    {
                        // Plain text - for InstallationTechnician, create multi-column search
                        if (entitySet == "InstallationTechnicianBaseInfos" && !string.IsNullOrWhiteSpace(radzenFilter))
                        {
                            var searchValue = radzenFilter.Trim().Trim('\'', '"');
                            var multiColumnFilter = $"(contains(Nom, '{searchValue}') or contains(Tel, '{searchValue}') or contains(Tel2, '{searchValue}') or contains(Tel3, '{searchValue}') or contains(Email, '{searchValue}') or contains(Description, '{searchValue}'))";
                            filters.Add(multiColumnFilter);
                            _logger.LogInformation("Created multi-column search filter: {Filter}", multiColumnFilter);
                        }
                        else
                        {
                            // For other entities, skip plain text filters (OData needs structured filters)
                            _logger.LogWarning("Plain text filter cannot be converted to OData. Filter: {Filter}", radzenFilter);
                        }
                    }
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

