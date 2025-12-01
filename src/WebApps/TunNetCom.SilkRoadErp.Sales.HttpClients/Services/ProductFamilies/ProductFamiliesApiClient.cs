using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProductFamilies;

public class ProductFamiliesApiClient : IProductFamiliesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductFamiliesApiClient> _logger;

    public ProductFamiliesApiClient(HttpClient httpClient, ILogger<ProductFamiliesApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<FamilleProduitResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/product-families", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.ReadJsonAsync<List<FamilleProduitResponse>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product families");
            throw;
        }
    }

    public async Task<OneOf<FamilleProduitResponse, BadRequestResponse>> CreateAsync(CreateFamilleProduitRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/product-families", request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return await response.ReadJsonAsync<FamilleProduitResponse>();
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return await response.ReadJsonAsync<BadRequestResponse>();
            }
            throw new Exception($"Unexpected response. Status Code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product family");
            throw;
        }
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(int id, UpdateFamilleProduitRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/product-families/{id}", request, cancellationToken);
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
            _logger.LogError(ex, "Error updating product family");
            throw;
        }
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/product-families/{id}", cancellationToken);
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
            _logger.LogError(ex, "Error deleting product family");
            throw;
        }
    }
}

