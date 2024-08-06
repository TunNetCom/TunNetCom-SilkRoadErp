using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;
namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;
public partial class ProviderService(HttpClient _httpClient)
{
    public async Task<PagedList<ProviderResponse>> GetProvidersAsync(int pageNumber, int pageSize, string searchKeyword = "")
    {
        var response = await _httpClient
            .GetAsync($"/Providers?pageNumber={pageNumber}&pageSize={pageSize}&searchKeyword={searchKeyword}");
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedProviders = JsonConvert.DeserializeObject<PagedList<ProviderResponse>>(responseContent);

        return pagedProviders;

    }

    public async Task<ProviderResponse> GetProviderByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/Providers/{id}");
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var provider = JsonConvert.DeserializeObject<ProviderResponse>(responseContent);

        return provider;
    }

    public async Task AddProviderAsync(CreateProviderRequest provider)
    {
        var response = await _httpClient.PostAsJsonAsync("/Providers", provider);
        await HandleResponse(response);
    }

    public async Task UpdateProviderAsync(int id, UpdateProviderRequest provider)
    {
        var response = await _httpClient.PutAsJsonAsync($"/Providers/{id}", provider);
        await HandleResponse(response);
    }

    public async Task DeleteProviderAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/Providers/{id}");
        await HandleResponse(response);
    }

    private async Task HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            throw new ClientServiceException(response.StatusCode, errorContent);
        }
    }
}
