namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services;

public partial class CustomerService(HttpClient _httpClient)
{
    public async Task<PagedList<CustomerResponse>>
        GetClientsAsync(int pageNumber, int pageSize, string searchKeyword = "")
    {
        var response = await _httpClient
            .GetAsync($"/customers?pageNumber={pageNumber}&pageSize={pageSize}&searchKeyword={searchKeyword}");
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedClients = JsonConvert.DeserializeObject<PagedList<CustomerResponse>>(responseContent);

        if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
        {
            var paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(headerValues.FirstOrDefault());
            return pagedClients;
        }

        return pagedClients;

    }

    public async Task<CustomerResponse>
        GetClientByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/customers/{id}");
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var client = JsonConvert.DeserializeObject<CustomerResponse>(responseContent);

        return client;
    }

    public async Task AddClientAsync(CreateCustomerRequest client)
    {
        var response = await _httpClient.PostAsJsonAsync("/customers", client);
        await HandleResponse(response);
    }

    public async Task UpdateClientAsync(int id, UpdateCustomerRequest client)
    {
        var response = await _httpClient.PutAsJsonAsync($"/customers/{id}", client);
        await HandleResponse(response);
    }

    public async Task DeleteClientAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/customers/{id}");
        await HandleResponse(response);
    }

    private async Task HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();


            //var validationErrors = JsonConvert.DeserializeObject<List<IError>>(errorContent);
            //throw new ClientServiceException(response.StatusCode, errorContent)
            //{
            //    Data = { ["ValidationErrors"] = validationErrors }
            //};


            throw new ClientServiceException(response.StatusCode, errorContent);
        }
    }
}