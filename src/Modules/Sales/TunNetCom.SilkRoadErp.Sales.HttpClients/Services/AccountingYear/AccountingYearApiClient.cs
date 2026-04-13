using TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

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

    public async Task<AccountingYearResponse?> GetAccountingYearByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching accounting year with Id {Id}", id);

        var response = await _httpClient.GetAsync($"accounting-years/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Accounting year with Id {Id} not found", id);
            return null;
        }

        _ = response.EnsureSuccessStatusCode();

        var accountingYear = await response.Content.ReadFromJsonAsync<AccountingYearResponse>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);

        _logger.LogInformation("Successfully retrieved accounting year: {Year}", accountingYear?.Year);
        return accountingYear;
    }

    public async Task<int> CreateAccountingYearAsync(CreateAccountingYearRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating accounting year for year {Year}", request.Year);

        var response = await _httpClient.PostAsJsonAsync("accounting-years", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var locationHeader = response.Headers.Location?.ToString();
        if (locationHeader != null)
        {
            var id = int.Parse(locationHeader.Split('/').Last());
            _logger.LogInformation("Successfully created accounting year with Id {Id}", id);
            return id;
        }
        return 0;
    }

    public async Task UpdateAccountingYearAsync(int id, UpdateAccountingYearRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating accounting year {Id}", id);

        var response = await _httpClient.PutAsJsonAsync($"accounting-years/{id}", request, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to update accounting year {Id}: {StatusCode} - {ErrorContent}", id, response.StatusCode, errorContent);
            throw new HttpRequestException($"Failed to update accounting year: {response.StatusCode} - {errorContent}");
        }

        _logger.LogInformation("Successfully updated accounting year {Id}", id);
    }

    public async Task DeleteAccountingYearAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting accounting year {Id}", id);

        var response = await _httpClient.DeleteAsync($"accounting-years/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();

        _logger.LogInformation("Successfully deleted accounting year {Id}", id);
    }
}

