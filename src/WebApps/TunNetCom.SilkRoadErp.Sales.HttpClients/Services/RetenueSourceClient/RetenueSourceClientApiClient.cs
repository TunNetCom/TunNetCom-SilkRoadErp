using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;
using TunNetCom.SilkRoadErp.Sales.HttpClients;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceClient;

public class RetenueSourceClientApiClient : IRetenueSourceClientApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RetenueSourceClientApiClient> _logger;

    public RetenueSourceClientApiClient(HttpClient httpClient, ILogger<RetenueSourceClientApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateRetenueSourceClientAsync(
        CreateRetenueSourceClientRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating retenue source client via API /retenue-source-client");
        var response = await _httpClient.PostAsJsonAsync("/retenue-source-client", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var retenueId))
                {
                    return retenueId;
                }
            }
            throw new Exception($"RetenueSourceClient: Unable to extract retenue id from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"RetenueSourceClient: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<RetenueSourceClientResponse>> GetRetenueSourceClientAsync(
        int numFacture,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching retenue source client from API /retenue-source-client/{numFacture}", numFacture);
        var response = await _httpClient.GetAsync($"/retenue-source-client/{numFacture}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var retenue = JsonConvert.DeserializeObject<RetenueSourceClientResponse>(responseContent);
            return Result.Ok(retenue!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_client_not_found");
        }

        throw new Exception($"RetenueSourceClient: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<byte[]>> GetRetenueSourceClientPdfAsync(
        int numFacture,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching retenue source client PDF from API /retenue-source-client/{numFacture}/pdf", numFacture);
        var response = await _httpClient.GetAsync($"/retenue-source-client/{numFacture}/pdf", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var pdfBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return Result.Ok(pdfBytes);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_client_not_found");
        }

        throw new Exception($"RetenueSourceClient: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> UpdateRetenueSourceClientAsync(
        int numFacture,
        UpdateRetenueSourceClientRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating retenue source client via API /retenue-source-client/{numFacture}", numFacture);
        var response = await _httpClient.PutAsJsonAsync($"/retenue-source-client/{numFacture}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_client_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"RetenueSourceClient: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> DeleteRetenueSourceClientAsync(
        int numFacture,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting retenue source client via API /retenue-source-client/{numFacture}", numFacture);
        var response = await _httpClient.DeleteAsync($"/retenue-source-client/{numFacture}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_client_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"RetenueSourceClient: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}


