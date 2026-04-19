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

        // Build absolute Uri: prefer HttpClient.BaseAddress when requestUri is relative
        Uri requestUriObj;
        if (Uri.IsWellFormedUriString(requestUri, UriKind.Absolute))
        {
            requestUriObj = new Uri(requestUri, UriKind.Absolute);
        }
        else if (_httpClient.BaseAddress != null)
        {
            requestUriObj = new Uri(_httpClient.BaseAddress, requestUri);
        }
        else
        {
            throw new InvalidOperationException($"Cannot send request: HttpClient.BaseAddress is not set and request URI is relative ('{requestUri}'). Register the HttpClient with a BaseAddress or use an absolute URI.");
        }

        var response = await _httpClient.GetAsync(requestUriObj, cancellationToken);
        //response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = System.Text.Json.JsonSerializer.Deserialize<GetProviderInvoicesWithSummary>(
                content,
                options);

            // Robust fallback: if items are missing in the deserialized PagedList, try to extract them manually
            if (result != null && (result.Invoices == null || result.Invoices.Items == null || !result.Invoices.Items.Any()))
            {
                try
                {
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("invoices", out var invoicesElem))
                    {
                        // Try common property names for the items array
                        JsonElement itemsElem;
                        if (invoicesElem.TryGetProperty("items", out itemsElem) ||
                            invoicesElem.TryGetProperty("Items", out itemsElem) ||
                            invoicesElem.TryGetProperty("data", out itemsElem))
                        {
                            if (itemsElem.ValueKind == JsonValueKind.Array)
                            {
                                var itemsJson = itemsElem.GetRawText();
                                var items = System.Text.Json.JsonSerializer.Deserialize<List<ProviderInvoiceResponse>>(itemsJson, options) ?? new List<ProviderInvoiceResponse>();
                                if (result.Invoices == null) result.Invoices = new PagedList<ProviderInvoiceResponse>();
                                result.Invoices.Items = items;

                                // Try to set total count if present
                                if (invoicesElem.TryGetProperty("totalCount", out var totalCountElem) && totalCountElem.TryGetInt32(out var tc))
                                {
                                    result.Invoices.TotalCount = tc;
                                }
                            }
                        }
                        else if (invoicesElem.ValueKind == JsonValueKind.Array)
                        {
                            // Some APIs return invoices as an array directly
                            var items = System.Text.Json.JsonSerializer.Deserialize<List<ProviderInvoiceResponse>>(invoicesElem.GetRawText(), options) ?? new List<ProviderInvoiceResponse>();
                            if (result.Invoices == null) result.Invoices = new PagedList<ProviderInvoiceResponse>();
                            result.Invoices.Items = items;
                        }
                    }
                    else
                    {
                        // Last-resort: scan the document for an array that looks like invoice items
                        JsonElement? candidateArray = null;
                        void Scan(JsonElement element)
                        {
                            if (candidateArray.HasValue) return;
                            if (element.ValueKind == JsonValueKind.Array)
                            {
                                var arr = element;
                                if (arr.GetArrayLength() > 0 && arr[0].ValueKind == JsonValueKind.Object && (arr[0].TryGetProperty("num", out _) || arr[0].TryGetProperty("id", out _)))
                                {
                                    candidateArray = arr;
                                    return;
                                }
                            }
                            else if (element.ValueKind == JsonValueKind.Object)
                            {
                                foreach (var prop in element.EnumerateObject())
                                {
                                    Scan(prop.Value);
                                    if (candidateArray.HasValue) return;
                                }
                            }
                        }

                        Scan(doc.RootElement);

                        if (candidateArray.HasValue)
                        {
                            var items = System.Text.Json.JsonSerializer.Deserialize<List<ProviderInvoiceResponse>>(candidateArray.Value.GetRawText(), options) ?? new List<ProviderInvoiceResponse>();
                            if (result.Invoices == null) result.Invoices = new PagedList<ProviderInvoiceResponse>();
                            result.Invoices.Items = items;
                        }
                    }
                }
                catch
                {
                    // ignore fallback errors and return whatever we have
                }
            }

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

    public async Task<Result> UpdateProviderInvoiceDateAsync(int num, UpdateProviderInvoiceDateRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"/provider-invoice/{num}/date", request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("invoice_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var bad = await response.ReadJsonAsync<BadRequestResponse>(cancellationToken: cancellationToken);
            return Result.Fail(bad?.Detail ?? "bad_request");
        }

        return Result.Fail($"Unexpected status code: {response.StatusCode}");
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
