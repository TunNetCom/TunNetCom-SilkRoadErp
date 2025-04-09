using System.Text.Json;
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

    public async Task<GetProviderInvoicesWithSummary> GetProvidersInvoicesAsync(
    int idFournisseur ,
    QueryStringParameters query ,
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
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<GetProviderInvoicesWithSummary>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?? new GetProviderInvoicesWithSummary();

    }
}
