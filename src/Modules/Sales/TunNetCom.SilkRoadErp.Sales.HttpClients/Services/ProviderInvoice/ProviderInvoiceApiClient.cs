using System.Text;
using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice;

public class ProviderInvoiceApiClient : IProviderInvoiceApiClient
{

    private readonly HttpClient _httpClient;
    private readonly ILogger<ProviderInvoiceApiClient> _logger;

    public ProviderInvoiceApiClient(HttpClient httpClient, ILogger<ProviderInvoiceApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<GetProviderInvoicesWithSummary, BadRequestResponse>> GetProvidersInvoicesAsync(
    int idFournisseur,
    QueryStringParameters query,
    CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "PageNumber", query.PageNumber.ToString() },
            { "PageSize", query.PageSize.ToString() }
        };

        queryParams.Add("IdFournisseur", idFournisseur.ToString());

        if (!string.IsNullOrEmpty(query.SearchKeyword))
        {
            queryParams.Add("SearchKeyword", query.SearchKeyword);
        }
        if (query.SortOrder != null && query.SortProprety != null)
        {
            queryParams.Add("SortOrder", query.SortOrder);
            queryParams.Add("SortProperty", query.SortProprety);
        }

        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        var requestUri = $"/provider-invoice?{queryString}";

        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        //response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var result = System.Text.Json.JsonSerializer.Deserialize<GetProviderInvoicesWithSummary>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result ?? new GetProviderInvoicesWithSummary();
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<List<ProviderInvoiceResponse>> GetProviderInvoicesByIdsAsync(
        List<int> invoicesIds,
        CancellationToken cancellationToken = default)
    {
        // Serialize the list of IDs to JSON
        var jsonContent = System.Text.Json.JsonSerializer.Serialize(invoicesIds);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Send POST request
        var response = await _httpClient.PostAsync("provider-invoices/byids", content, cancellationToken);

        // Ensure the response is successful
        _ = response.EnsureSuccessStatusCode();

        // Deserialize the response
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = System.Text.Json.JsonSerializer.Deserialize<List<ProviderInvoiceResponse>>(responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result ;
    }

    public async Task<Result<FullProviderInvoiceResponse>> GetFullProviderInvoiceByIdAsync(
    int id,
    CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/provider-invoices/{id}/full",
            cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("invoice_not_found");
        }

        _ = response.EnsureSuccessStatusCode();

        var invoice = await response.Content.ReadFromJsonAsync<FullProviderInvoiceResponse>(
            cancellationToken: cancellationToken);

        return invoice is null
            ? Result.Fail<FullProviderInvoiceResponse>("invoice_is_empty")
            : Result.Ok(invoice);
    }

    public async Task<OneOf<int, BadRequestResponse>> CreateProviderInvoice(
        CreateProviderInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("provider-invoice", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            // Extract invoice number from Location header (format: /provider-invoice/{number})
            if (response.Headers.Location != null)
            {
                var locationPath = response.Headers.Location.ToString();
                var segments = locationPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2 && int.TryParse(segments[segments.Length - 1], out var invoiceNumber))
                {
                    return invoiceNumber;
                }
            }
            throw new Exception($"ProviderInvoice: Unable to extract invoice number from Location header: {response.Headers.Location}");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"ProviderInvoice: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateProviderInvoiceAsync(
        int num,
        UpdateProviderInvoiceRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var headers = new Dictionary<string, string>()
            {
                { $"Accept", $"application/problem+json" }
            };

            var response = await _httpClient.PutAsJsonAsync($"provider-invoice/{num}", request, headers, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return ResponseTypes.Success;
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return ResponseTypes.NotFound;
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return await response.ReadJsonAsync<BadRequestResponse>();
            }
            throw new Exception($"UpdateProviderInvoice: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<Result> ValidateProviderInvoicesAsync(List<int> ids, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating provider invoices via API api/provider-invoices/validate");
        var request = new ValidateProviderInvoicesRequest { Ids = ids };
        var response = await _httpClient.PostAsJsonAsync("api/provider-invoices/validate", request, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("provider_invoices_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail(badRequest.Detail ?? badRequest.Title ?? "Unknown error");
        }

        throw new Exception($"Provider invoices validation: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}
