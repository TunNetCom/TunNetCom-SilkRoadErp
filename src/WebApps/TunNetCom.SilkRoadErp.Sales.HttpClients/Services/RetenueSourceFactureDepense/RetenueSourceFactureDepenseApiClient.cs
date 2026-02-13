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
        var response = await _httpClient.PostAsJsonAsync("/retenue-source-facture-depense", request, cancellationToken: cancellationToken);

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
            return 0;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"RetenueSourceFactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<RetenueSourceFactureDepenseResponse>> GetRetenueSourceFactureDepenseAsync(
        int factureDepenseId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/retenue-source-facture-depense/{factureDepenseId}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var retenue = JsonConvert.DeserializeObject<RetenueSourceFactureDepenseResponse>(responseContent);
            if (retenue == null)
            {
                return Result.Fail("retenue_source_facture_depense_not_found");
            }
            return Result.Ok(retenue);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_facture_depense_not_found");
        }

        throw new Exception($"RetenueSourceFactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<byte[]>> GetRetenueSourceFactureDepensePdfAsync(
        int factureDepenseId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/retenue-source-facture-depense/{factureDepenseId}/pdf", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var pdfBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return Result.Ok(pdfBytes);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_facture_depense_not_found");
        }

        throw new Exception($"RetenueSourceFactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> UpdateRetenueSourceFactureDepenseAsync(
        int factureDepenseId,
        UpdateRetenueSourceFactureDepenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"/retenue-source-facture-depense/{factureDepenseId}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("retenue_source_facture_depense_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"RetenueSourceFactureDepense: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}
