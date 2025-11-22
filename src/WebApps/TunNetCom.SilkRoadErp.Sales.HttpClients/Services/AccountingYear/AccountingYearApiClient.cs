namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AccountingYear;

public class AccountingYearApiClient : IAccountingYearApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AccountingYearApiClient> _logger;

    public AccountingYearApiClient(HttpClient httpClient, ILogger<AccountingYearApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GetActiveAccountingYearResponse?> GetActiveAccountingYearAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching active accounting year");

        var response = await _httpClient.GetAsync("accountingYear/active", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("No active accounting year found");
            return null;
        }

        _ = response.EnsureSuccessStatusCode();

        var activeYear = await response.Content.ReadFromJsonAsync<GetActiveAccountingYearResponse>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);

        _logger.LogInformation("Successfully retrieved active accounting year: {Year}", activeYear?.Year);
        return activeYear;
    }

    public async Task<List<GetAllAccountingYearsResponse>> GetAllAccountingYearsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all accounting years");

        var response = await _httpClient.GetAsync("accountingYear", cancellationToken);

        _ = response.EnsureSuccessStatusCode();

        var years = await response.Content.ReadFromJsonAsync<List<GetAllAccountingYearsResponse>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);

        _logger.LogInformation("Successfully retrieved {Count} accounting years", years?.Count ?? 0);
        return years ?? new List<GetAllAccountingYearsResponse>();
    }

    public async Task<bool> SetActiveAccountingYearAsync(int accountingYearId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Setting accounting year {Id} as active", accountingYearId);

        var request = new { AccountingYearId = accountingYearId };
        var response = await _httpClient.PostAsJsonAsync("accountingYear/active", request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("Failed to set accounting year {Id} as active", accountingYearId);
            return false;
        }

        _ = response.EnsureSuccessStatusCode();

        _logger.LogInformation("Successfully set accounting year {Id} as active", accountingYearId);
        return true;
    }
}

