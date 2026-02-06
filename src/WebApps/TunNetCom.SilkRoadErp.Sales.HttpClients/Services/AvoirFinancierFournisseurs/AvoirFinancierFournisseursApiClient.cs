using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFinancierFournisseurs;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.AvoirFinancierFournisseurs;

public class AvoirFinancierFournisseursApiClient : IAvoirFinancierFournisseursApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AvoirFinancierFournisseursApiClient> _logger;

    public AvoirFinancierFournisseursApiClient(HttpClient httpClient, ILogger<AvoirFinancierFournisseursApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateAvoirFinancierFournisseurs(
        CreateAvoirFinancierFournisseursRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating avoir financier fournisseurs via API /avoir-financier-fournisseurs");
        var response = await _httpClient.PostAsJsonAsync("/avoir-financier-fournisseurs", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var num))
                {
                    return num;
                }
            }
            throw new Exception($"AvoirFinancierFournisseurs: Unable to extract number from Location header: {response.Headers.Location}");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"AvoirFinancierFournisseurs: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<AvoirFinancierFournisseursResponse>> GetAvoirFinancierFournisseursAsync(
        int num,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching avoir financier fournisseurs from API /avoir-financier-fournisseurs/{num}", num);
        var response = await _httpClient.GetAsync($"/avoir-financier-fournisseurs/{num}", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var avoir = JsonConvert.DeserializeObject<AvoirFinancierFournisseursResponse>(responseContent);
            return Result.Ok(avoir!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_financier_fournisseurs_not_found");
        }

        throw new Exception($"AvoirFinancierFournisseurs: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result<FullAvoirFinancierFournisseursResponse>> GetFullAvoirFinancierFournisseursAsync(
        int num,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching full avoir financier fournisseurs from API /avoir-financier-fournisseurs/{num}/full", num);
        var response = await _httpClient.GetAsync($"/avoir-financier-fournisseurs/{num}/full", cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var avoir = JsonConvert.DeserializeObject<FullAvoirFinancierFournisseursResponse>(responseContent);
            return Result.Ok(avoir!);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_financier_fournisseurs_not_found");
        }

        throw new Exception($"AvoirFinancierFournisseurs: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<GetAvoirFinancierFournisseursWithSummariesResponse> GetAvoirFinancierFournisseursWithSummariesAsync(
        int? providerId,
        int? numFactureFournisseur,
        string? sortOrder,
        string? sortProperty,
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        DateTime? startDate,
        DateTime? endDate,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching avoir financier fournisseurs with summaries from API /avoir-financier-fournisseurs");
        var queryString = $"/avoir-financier-fournisseurs?pageNumber={pageNumber}&pageSize={pageSize}";

        if (providerId.HasValue)
        {
            queryString += $"&providerId={providerId.Value}";
        }

        if (numFactureFournisseur.HasValue)
        {
            queryString += $"&numFactureFournisseur={numFactureFournisseur.Value}";
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
        var result = JsonConvert.DeserializeObject<GetAvoirFinancierFournisseursWithSummariesResponse>(responseContent);
        return result!;
    }

    public async Task<Result> UpdateAvoirFinancierFournisseursAsync(
        int num,
        UpdateAvoirFinancierFournisseursRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating avoir financier fournisseurs via API /avoir-financier-fournisseurs/{num}", num);
        var response = await _httpClient.PutAsJsonAsync($"/avoir-financier-fournisseurs/{num}", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_financier_fournisseurs_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"AvoirFinancierFournisseurs: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> ValidateAvoirFinancierFournisseursAsync(List<int> ids, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating avoir financier fournisseurs via API /avoir-financier-fournisseurs/validate");
        var request = new ValidateAvoirFinancierFournisseursRequest { Ids = ids };
        var response = await _httpClient.PostAsJsonAsync("/avoir-financier-fournisseurs/validate", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"AvoirFinancierFournisseurs: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> AttachAvoirFinancierToInvoiceAsync(int id, int numFactureFournisseur, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attaching avoir financier {Id} to invoice {NumFactureFournisseur} via API", id, numFactureFournisseur);
        var request = new AttachAvoirFinancierToInvoiceRequest { NumFactureFournisseur = numFactureFournisseur };
        var response = await _httpClient.PutAsJsonAsync($"/avoir-financier-fournisseurs/{id}/attach", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_financier_not_found");
        }

        throw new Exception($"AvoirFinancierFournisseurs Attach: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<Result> DetachAvoirFinancierFromInvoiceAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Detaching avoir financier {Id} from invoice via API", id);
        var response = await _httpClient.PostAsync($"/avoir-financier-fournisseurs/{id}/detach", null, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("avoir_financier_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail($"validation_error: {JsonConvert.SerializeObject(badRequest)}");
        }

        throw new Exception($"AvoirFinancierFournisseurs Detach: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}

