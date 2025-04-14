using System.Text;
using System.Text.Json;
using FluentResults;
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
        response.EnsureSuccessStatusCode();

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

        response.EnsureSuccessStatusCode();

        var invoice = await response.Content.ReadFromJsonAsync<FullProviderInvoiceResponse>(
            cancellationToken: cancellationToken);

        return invoice is null
            ? Result.Fail<FullProviderInvoiceResponse>("invoice_is_empty")
            : Result.Ok(invoice);
    }
}
