using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;
using TunNetCom.SilkRoadErp.Sales.HttpClients;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceFactureDepense;

public class RetenueSourceFactureDepenseApiClient : IRetenueSourceFactureDepenseApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RetenueSourceFactureDepenseApiClient> _logger;

    public RetenueSourceFactureDepenseApiClient(HttpClient httpClient, ILogger<RetenueSourceFactureDepenseApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateRetenueSourceFactureDepenseAsync(
        CreateRetenueSourceFactureDepenseRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating RetenueSourceFactureDepense with request: {@Request}", request);

        var response = await _httpClient.PostAsJsonAsync("/retenue-source-facture-depense", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var retenueId))
                {
                    _logger.LogInformation("RetenueSourceFactureDepense created successfully with ID: {Id}", retenueId);
                    return retenueId;
                }
            }
            _logger.LogWarning("RetenueSourceFactureDepense created but unable to extract ID from Location header");
            return 0;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("RetenueSourceFactureDepense create returned BadRequest: {@Request}", request);
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        _logger.LogError("Unexpected response creating RetenueSourceFactureDepense: {StatusCode}", response.StatusCode);
        throw new Exception($"RetenueSourceFactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<RetenueSourceFactureDepenseResponse>> GetRetenueSourceFactureDepenseAsync(
        int factureDepenseId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching RetenueSourceFactureDepense for FactureDepense ID: {Id}", factureDepenseId);

        var response = await _httpClient.GetAsync($"/retenue-source-facture-depense/{factureDepenseId}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var retenue = JsonConvert.DeserializeObject<RetenueSourceFactureDepenseResponse>(responseContent);
            if (retenue == null)
            {
                _logger.LogWarning("RetenueSourceFactureDepense for FactureDepense {Id} deserialized to null", factureDepenseId);
                return Result.Fail("retenue_source_facture_depense_not_found");
            }
            _logger.LogInformation("Fetched RetenueSourceFactureDepense for FactureDepense ID: {Id}", factureDepenseId);
            return Result.Ok(retenue);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("RetenueSourceFactureDepense for FactureDepense {Id} not found", factureDepenseId);
            return Result.Fail("retenue_source_facture_depense_not_found");
        }

        _logger.LogError("Unexpected response fetching RetenueSourceFactureDepense {Id}: {StatusCode}", factureDepenseId, response.StatusCode);
        throw new Exception($"RetenueSourceFactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<byte[]>> GetRetenueSourceFactureDepensePdfAsync(
        int factureDepenseId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching RetenueSourceFactureDepense PDF for FactureDepense ID: {Id}", factureDepenseId);

        var response = await _httpClient.GetAsync($"/retenue-source-facture-depense/{factureDepenseId}/pdf", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var pdfBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            _logger.LogInformation("Fetched RetenueSourceFactureDepense PDF for FactureDepense {Id} ({Size} bytes)", factureDepenseId, pdfBytes.Length);
            return Result.Ok(pdfBytes);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("RetenueSourceFactureDepense PDF for FactureDepense {Id} not found", factureDepenseId);
            return Result.Fail("retenue_source_facture_depense_not_found");
        }

        _logger.LogError("Unexpected response fetching RetenueSourceFactureDepense PDF {Id}: {StatusCode}", factureDepenseId, response.StatusCode);
        throw new Exception($"RetenueSourceFactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> UpdateRetenueSourceFactureDepenseAsync(
        int factureDepenseId,
        UpdateRetenueSourceFactureDepenseRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating RetenueSourceFactureDepense for FactureDepense ID: {Id}", factureDepenseId);

        var response = await _httpClient.PutAsJsonAsync($"/retenue-source-facture-depense/{factureDepenseId}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            _logger.LogInformation("RetenueSourceFactureDepense for FactureDepense {Id} updated successfully", factureDepenseId);
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("RetenueSourceFactureDepense for FactureDepense {Id} not found for update", factureDepenseId);
            return Result.Fail("retenue_source_facture_depense_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            _logger.LogWarning("RetenueSourceFactureDepense update {Id} returned BadRequest", factureDepenseId);
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        _logger.LogError("Unexpected response updating RetenueSourceFactureDepense {Id}: {StatusCode}", factureDepenseId, response.StatusCode);
        throw new Exception($"RetenueSourceFactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}
