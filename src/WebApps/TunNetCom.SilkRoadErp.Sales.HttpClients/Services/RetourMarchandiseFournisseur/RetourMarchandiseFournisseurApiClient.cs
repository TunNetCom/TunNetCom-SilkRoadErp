using System.Net;
using TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.RetourMarchandiseFournisseur;

class RetourMarchandiseFournisseurApiClient : IRetourMarchandiseFournisseurApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RetourMarchandiseFournisseurApiClient> _logger;

    public RetourMarchandiseFournisseurApiClient(HttpClient httpClient, ILogger<RetourMarchandiseFournisseurApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<int>> CreateRetourMarchandiseFournisseurAsync(
        CreateRetourMarchandiseFournisseurRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/retour-marchandise-fournisseur",
                request,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                // Response is { "num": <number> }
                var jsonDoc = System.Text.Json.JsonDocument.Parse(content);
                if (jsonDoc.RootElement.TryGetProperty("num", out var numElement))
                {
                    var num = numElement.GetInt32();
                    return Result.Ok(num);
                }
                // Fallback: try to parse as int directly
                if (int.TryParse(content, out var directNum))
                {
                    return Result.Ok(directNum);
                }
                return Result.Fail("invalid_response_format");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Bad request when creating retour: {Error}", errorContent);
                return Result.Fail("bad_request");
            }

            response.EnsureSuccessStatusCode();
            return Result.Fail("unexpected_error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating retour marchandise fournisseur");
            return Result.Fail("error_creating_retour");
        }
    }

    public async Task<Result> UpdateRetourMarchandiseFournisseurAsync(
        UpdateRetourMarchandiseFournisseurRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(
                "/retour-marchandise-fournisseur",
                request,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Result.Ok();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Bad request when updating retour: {Error}", errorContent);
                return Result.Fail("bad_request");
            }

            response.EnsureSuccessStatusCode();
            return Result.Fail("unexpected_error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating retour marchandise fournisseur");
            return Result.Fail("error_updating_retour");
        }
    }

    public async Task<Result<RetourMarchandiseFournisseurResponse>> GetRetourMarchandiseFournisseurAsync(
        int num,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/retour-marchandise-fournisseur/{num}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.Fail("not_found");
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var retour = System.Text.Json.JsonSerializer.Deserialize<RetourMarchandiseFournisseurResponse>(
                content,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Result.Ok(retour);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting retour marchandise fournisseur {Num}", num);
            return Result.Fail("error_getting_retour");
        }
    }

    public async Task<Result> ValidateRetourMarchandiseFournisseurAsync(
        List<int> ids,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ValidateRetourMarchandiseFournisseurRequest { Ids = ids };
            var response = await _httpClient.PostAsJsonAsync(
                "/retour-marchandise-fournisseur/validate",
                request,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Result.Ok();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Bad request when validating retours: {Error}", errorContent);
                return Result.Fail("bad_request");
            }

            response.EnsureSuccessStatusCode();
            return Result.Fail("unexpected_error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating retours marchandise fournisseur");
            return Result.Fail("error_validating_retours");
        }
    }
}

