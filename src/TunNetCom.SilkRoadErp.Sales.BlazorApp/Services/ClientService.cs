using TunNetCom.SilkRoadErp.Sales.Api.Contracts.Client;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Utilities;

namespace TunNetCom.SilkRoadErp.Sales.BlazorApp.Services;

public class ClientService(HttpClient httpClient)
{
   

    public async Task<(List<ClientResponse> Clients, int TotalCount)> GetClientsAsync(int pageIndex, int pageSize)
    {
        

        var response = await httpClient.GetFromJsonAsync<PagedList<ClientResponse>>($"/clients?pageIndex={pageIndex}&pageSize={pageSize}");

        if (response == null)
        {
           
            return (new List<ClientResponse>(), 0);
        }

        

        return (response, response.TotalCount);
    }

    public async Task<ClientResponse> GetClientByIdAsync(int id)
    {
        

        var client = await httpClient.GetFromJsonAsync<ClientResponse>($"/clients/{id}");

       

        return client;
    }

    public async Task AddClientAsync(CreateClientRequest client)
    {
       

        var response = await httpClient.PostAsJsonAsync("/clients", client);
        await HandleResponse(response);

       
    }

    public async Task UpdateClientAsync(int id, UpdateClientRequest client)
    {
        

        var response = await httpClient.PutAsJsonAsync($"/clients/{id}", client);
        await HandleResponse(response);

        
    }

    public async Task DeleteClientAsync(int id)
    {
       

        var response = await httpClient.DeleteAsync($"/clients/{id}");
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
