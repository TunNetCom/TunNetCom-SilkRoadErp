using Newtonsoft.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Inventaire;

public class InventaireApiClient : IInventaireApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventaireApiClient> _logger;

    public InventaireApiClient(HttpClient httpClient, ILogger<InventaireApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedList<InventaireResponse>> GetPagedAsync(GetInventairesQueryParams queryParameters, CancellationToken cancellationToken)
    {
        var queryString = $"/inventaires?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}";
        if (!string.IsNullOrEmpty(queryParameters.SearchKeyword))
        {
            queryString += $"&searchKeyword={Uri.EscapeDataString(queryParameters.SearchKeyword)}";
        }
        if (!string.IsNullOrEmpty(queryParameters.SortProperty))
        {
            queryString += $"&sortProperty={queryParameters.SortProperty}&sortOrder={queryParameters.SortOrder ?? "asc"}";
        }
        if (queryParameters.AccountingYearId.HasValue)
        {
            queryString += $"&accountingYearId={queryParameters.AccountingYearId.Value}";
        }

        var response = await _httpClient.GetAsync(queryString, cancellationToken: cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var pagedInventaires = JsonConvert.DeserializeObject<PagedList<InventaireResponse>>(responseContent);

        return pagedInventaires ?? new PagedList<InventaireResponse>();
    }

    public async Task<OneOf<FullInventaireResponse, bool>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/inventaires/{id}", cancellationToken: cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.ReadJsonAsync<FullInventaireResponse>();
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            throw new Exception($"Inventaires/{id}: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<OneOf<CreateInventaireRequest, BadRequestResponse>> CreateAsync(CreateInventaireRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync($"/inventaires", request, cancellationToken: cancellationToken);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            return await response.ReadJsonAsync<CreateInventaireRequest>();
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }
        throw new Exception($"Inventaires: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(UpdateInventaireRequest request, int id, CancellationToken cancellationToken)
    {
        try
        {
            var headers = new Dictionary<string, string>()
            {
                { "Accept", "application/problem+json" }
            };

            var response = await _httpClient.PutAsJsonAsync($"/inventaires/{id}", request, headers, cancellationToken);
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
            throw new Exception($"Inventaires/{id}: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<OneOf<ResponseTypes, Stream>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/inventaires/{id}", cancellationToken: cancellationToken);
            if (response.StatusCode is HttpStatusCode.NoContent)
            {
                return await response.Content.ReadAsStreamAsync(cancellationToken);
            }
            if (response.StatusCode is HttpStatusCode.NotFound)
            {
                return ResponseTypes.NotFound;
            }
            throw new Exception($"Inventaires/{id}: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> ValiderAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/inventaires/{id}/valider", null, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return ResponseTypes.Success;
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return await response.ReadJsonAsync<BadRequestResponse>();
            }
            throw new Exception($"Inventaires/{id}/valider: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<OneOf<ResponseTypes, BadRequestResponse>> CloturerAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/inventaires/{id}/cloturer", null, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return ResponseTypes.Success;
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return await response.ReadJsonAsync<BadRequestResponse>();
            }
            throw new Exception($"Inventaires/{id}/cloturer: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<OneOf<decimal, bool>> GetDernierPrixAchatAsync(string refProduit, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/inventaires/dernier-prix-achat/{Uri.EscapeDataString(refProduit)}", cancellationToken: cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                if (result != null && result.dernierPrixAchat != null)
                {
                    var prixAchat = result.dernierPrixAchat;
                    if (prixAchat != null)
                    {
                        return (decimal)prixAchat;
                    }
                }
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            throw new Exception($"Inventaires/dernier-prix-achat/{refProduit}: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<OneOf<List<HistoriqueAchatVenteResponse>, bool>> GetHistoriqueAchatVenteAsync(string refProduit, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/inventaires/historique-achat-vente/{Uri.EscapeDataString(refProduit)}", cancellationToken: cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.ReadJsonAsync<List<HistoriqueAchatVenteResponse>>();
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            throw new Exception($"Inventaires/historique-achat-vente/{refProduit}: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }
}

