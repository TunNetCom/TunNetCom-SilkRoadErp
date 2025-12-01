using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProductSubFamilies;

public class ProductSubFamiliesApiClient : IProductSubFamiliesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductSubFamiliesApiClient> _logger;

    public ProductSubFamiliesApiClient(HttpClient httpClient, ILogger<ProductSubFamiliesApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<SousFamilleProduitResponse>> GetAllAsync(int? familleProduitId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = "/product-subfamilies";
            if (familleProduitId.HasValue)
            {
                url += $"?familleProduitId={familleProduitId.Value}";
            }
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.ReadJsonAsync<List<SousFamilleProduitResponse>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product subfamilies");
            throw;
        }
    }

    public async Task<OneOf<SousFamilleProduitResponse, BadRequestResponse>> CreateAsync(CreateSousFamilleProduitRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/product-subfamilies", request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return await response.ReadJsonAsync<SousFamilleProduitResponse>();
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return await response.ReadJsonAsync<BadRequestResponse>();
            }
            throw new Exception($"Unexpected response. Status Code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product subfamily");
            throw;
        }
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(int id, UpdateSousFamilleProduitRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/product-subfamilies/{id}", request, cancellationToken);
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
            throw new Exception($"Unexpected response. Status Code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product subfamily");
            throw;
        }
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/product-subfamilies/{id}", cancellationToken);
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
            throw new Exception($"Unexpected response. Status Code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product subfamily");
            throw;
        }
    }
}

