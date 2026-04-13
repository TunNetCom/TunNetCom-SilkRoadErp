using TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetenueSourceFournisseur;

public class RetenueSourceFournisseurApiClient : IRetenueSourceFournisseurApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RetenueSourceFournisseurApiClient> _logger;

    public RetenueSourceFournisseurApiClient(HttpClient httpClient, ILogger<RetenueSourceFournisseurApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateRetenueSourceFournisseurAsync(
        CreateRetenueSourceFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating retenue source fournisseur via API /retenue-source-fournisseur");
        var response = await _httpClient.PostAsJsonAsync("/retenue-source-fournisseur", request, cancellationToken: cancellationToken);

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
            throw new Exception($"RetenueSourceFournisseur: Unable to extract retenue id from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"RetenueSourceFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<RetenueSourceFournisseurResponse>> GetRetenueSourceFournisseurAsync(
        int numFactureFournisseur,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching retenue source fournisseur from API /retenue-source-fournisseur/{numFactureFournisseur}", numFactureFournisseur);
        var response = await _httpClient.GetAsync($"/retenue-source-fournisseur/{numFactureFournisseur}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var retenue = JsonConvert.DeserializeObject<RetenueSourceFournisseurResponse>(responseContent);
            if (retenue == null)
            {
                _logger.LogWarning("API returned 200 OK but deserialized object is null for FactureFournisseur {NumFactureFournisseur}", numFactureFournisseur);
                return Result.Fail("retenue_source_fournisseur_not_found");
            }
            return Result.Ok(retenue);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_fournisseur_not_found");
        }

        throw new Exception($"RetenueSourceFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<byte[]>> GetRetenueSourceFournisseurPdfAsync(
        int numFactureFournisseur,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching retenue source fournisseur PDF from API /retenue-source-fournisseur/{numFactureFournisseur}/pdf", numFactureFournisseur);
        var response = await _httpClient.GetAsync($"/retenue-source-fournisseur/{numFactureFournisseur}/pdf", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var pdfBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return Result.Ok(pdfBytes);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_fournisseur_not_found");
        }

        throw new Exception($"RetenueSourceFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> UpdateRetenueSourceFournisseurAsync(
        int numFactureFournisseur,
        UpdateRetenueSourceFournisseurRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating retenue source fournisseur via API /retenue-source-fournisseur/{numFactureFournisseur}", numFactureFournisseur);
        var response = await _httpClient.PutAsJsonAsync($"/retenue-source-fournisseur/{numFactureFournisseur}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_fournisseur_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"RetenueSourceFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> DeleteRetenueSourceFournisseurAsync(
        int numFactureFournisseur,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting retenue source fournisseur via API /retenue-source-fournisseur/{numFactureFournisseur}", numFactureFournisseur);
        var response = await _httpClient.DeleteAsync($"/retenue-source-fournisseur/{numFactureFournisseur}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_fournisseur_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"RetenueSourceFournisseur: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}


