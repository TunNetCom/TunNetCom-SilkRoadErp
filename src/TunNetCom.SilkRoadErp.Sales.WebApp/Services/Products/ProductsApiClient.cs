namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Product;

public class ProductsApiClient : IProductsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductsApiClient> _logger;

    public ProductsApiClient(HttpClient httpClient, ILogger<ProductsApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OneOf<ProductResponse, bool>> GetAsync(string refe, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/products/{refe}", cancellationToken: cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.ReadJsonAsync<ProductResponse>();
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            throw new Exception($"Products/{refe}: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<PagedList<ProductResponse>> GetPagedAsync(QueryStringParameters queryParameters, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"/products?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}&searchKeyword={queryParameters.SearchKeyword}",
            cancellationToken: cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedProducts = JsonConvert.DeserializeObject<PagedList<ProductResponse>>(responseContent);

        if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
        {
            var paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(headerValues.FirstOrDefault());

            pagedProducts.TotalCount = paginationMetadata.TotalCount;
            pagedProducts.PageSize = paginationMetadata.PageSize;
            pagedProducts.TotalPages = paginationMetadata.TotalPages;
            pagedProducts.CurrentPage = paginationMetadata.CurrentPage;

            return pagedProducts;
        }
        return pagedProducts;
    }

    public async Task<OneOf<CreateProductRequest, BadRequestResponse>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync($"", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            return await response.ReadJsonAsync<CreateProductRequest>();
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"Products: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(UpdateProductRequest request, string refe, CancellationToken cancellationToken)
    {
        try
        {
            var headers = new Dictionary<string, string>()
                {
                    { "Accept", "application/problem+json" }
                };

            var response = await _httpClient.PutAsJsonAsync($"{refe}", request, headers, cancellationToken);
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
            throw new Exception($"Products/{refe}: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<OneOf<ResponseTypes, Stream>> DeleteAsync(string refe, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{refe}", cancellationToken: cancellationToken);
            if (response.StatusCode is HttpStatusCode.NoContent)
            {
                return await response.Content.ReadAsStreamAsync(cancellationToken);
            }
            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return ResponseTypes.NotFound;
            }
            throw new Exception($"Products/{refe}: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }


    public async Task<PagedList<ProductResponse>> SearchProducts(QueryStringParameters queryParameters, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/products?searchKeyword={queryParameters.SearchKeyword}&pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}", cancellationToken);
        var jsonResponse = await response.Content.ReadAsStringAsync();

        var pagedProducts = JsonConvert.DeserializeObject<PagedList<ProductResponse>>(jsonResponse);
        return pagedProducts;
    }

}
