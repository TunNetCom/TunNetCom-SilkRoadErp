using TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Avoirs;

public class AvoirsApiClient : IAvoirsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AvoirsApiClient> _logger;

    public AvoirsApiClient(HttpClient httpClient, ILogger<AvoirsApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateAvoir(
        CreateAvoirRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating avoir via API /avoirs");
        var response = await _httpClient.PostAsJsonAsync("/avoirs", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var avoirNum))
                {
                    return avoirNum;
                }
            }
            throw new Exception($"Avoirs: Unable to extract avoir number from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"Avoirs: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<AvoirResponse>> GetAvoirAsync(
        int num,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching avoir from API /avoirs/{num}", num);
        var response = await _httpClient.GetAsync($"/avoirs/{num}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var avoir = JsonConvert.DeserializeObject<AvoirResponse>(responseContent);
            return Result.Ok(avoir!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_not_found");
        }

        throw new Exception($"Avoirs: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<FullAvoirResponse>> GetFullAvoirAsync(
        int num,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching full avoir from API /avoirs/{num}/full", num);
        var response = await _httpClient.GetAsync($"/avoirs/{num}/full", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var avoir = JsonConvert.DeserializeObject<FullAvoirResponse>(responseContent);
            return Result.Ok(avoir!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_not_found");
        }

        throw new Exception($"Avoirs: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<GetAvoirsWithSummariesResponse> GetAvoirsWithSummariesAsync(
        int? clientId,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken,
        int? status = null)
    {
        _logger.LogInformation("Fetching avoirs with summaries from API /avoirs/summaries");
        var queryString = $"/avoirs/summaries?pageNumber={pageNumber}&pageSize={pageSize}";

        if (status.HasValue)
        {
            queryString += $"&status={status.Value}";
        }

        if (clientId.HasValue)
        {
            queryString += $"&clientId={clientId.Value}";
        }

        if (!string.IsNullOrEmpty(sortOrder))
        {
            queryString += $"&sortOrder={Uri.EscapeDataString(sortOrder)}";
        }

        if (!string.IsNullOrEmpty(sortProperty))
        {
            queryString += $"&sortProperty={Uri.EscapeDataString(sortProperty)}";
        }

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            queryString += $"&searchKeyword={Uri.EscapeDataString(searchKeyword)}";
        }

        if (startDate.HasValue)
        {
            queryString += $"&startDate={startDate.Value:yyyy-MM-dd}";
        }

        if (endDate.HasValue)
        {
            queryString += $"&endDate={endDate.Value:yyyy-MM-dd}";
        }

        var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetAvoirsWithSummariesResponse>(responseContent);
        return result!;
    }

    public async Task<Result> UpdateAvoirAsync(
        int num,
        UpdateAvoirRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating avoir via API /avoirs/{num}", num);
        var response = await _httpClient.PutAsJsonAsync($"/avoirs/{num}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"Avoirs: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> ValidateAvoirsAsync(List<int> ids, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating avoirs via API /avoirs/validate");
        var response = await _httpClient.PostAsJsonAsync("/avoirs/validate", ids, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail(badRequest.Detail ?? badRequest.Title ?? "Unknown error");
        }

        throw new Exception($"Avoirs validation: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}

