using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TunNetCom.SilkRoadErp.Sales.Api.Contracts.Client;
using TunNetCom.SilkRoadErp.Sales.Api.Infrastructure.Utilities;

namespace TunNetCom.SilkRoadErp.Sales.BlazorApp.Services;

public class ClientService(HttpClient _httpClient)
{
    public async Task<(List<ClientResponse> Clients, int TotalCount, int CurrentPage, int TotalPages)> 
        GetClientsAsync(int pageNumber, int pageSize, string searchKeyword = "")
    {
        var response = await _httpClient.GetAsync($"/clients?pageNumber={pageNumber}&pageSize={pageSize}&searchKeyword={searchKeyword}");
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var pagedClients = JsonConvert.DeserializeObject<PagedList<ClientResponse>>(responseContent);

        if (pagedClients is null)
        {
            return (new List<ClientResponse>(), 0, 1, 1);
        }

        if (response.Headers.TryGetValues("X-Pagination", out var headerValues))
            {
                var paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(headerValues.FirstOrDefault());
                return (pagedClients, paginationMetadata.TotalCount, paginationMetadata.CurrentPage, paginationMetadata.TotalPages);
        }

        return (pagedClients, pagedClients.TotalCount, pagedClients.CurrentPage, pagedClients.TotalPages);
        
    }

    public async Task<ClientResponse> 
        GetClientByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/clients/{id}");
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var client = JsonConvert.DeserializeObject<ClientResponse>(responseContent);

        return client;
    }

    public async Task 
        AddClientAsync(CreateClientRequest client)
    {
        var response = await _httpClient.PostAsJsonAsync("/clients", client);
        await HandleResponse(response);
    }

    public async Task 
        UpdateClientAsync(int id, UpdateClientRequest client)
    {
        var response = await _httpClient.PutAsJsonAsync($"/clients/{id}", client);
        await HandleResponse(response);
    }

    public async Task 
        DeleteClientAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/clients/{id}");
        await HandleResponse(response);
    }

    private async Task 
        HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            {
                try
                {
                    var validationErrors = JsonConvert.DeserializeObject<List<IError>>(errorContent);
                    throw new ClientServiceException(response.StatusCode, errorContent)
                    {
                        Data = { ["ValidationErrors"] = validationErrors }
                    };
                }
                catch (JsonException)
                {
                   
                }
            }

            throw new ClientServiceException(response.StatusCode, errorContent);
        }
    }

    public async Task<Dictionary<string, List<string>>> 
        ValidateFieldAsync(string fieldName, ClientResponse client)
    {
        var response = await _httpClient.PostAsJsonAsync($"/clients/validate/{fieldName}", client);

        if (!response.IsSuccessStatusCode)
        {
            throw new ClientServiceException(response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var validationErrors = await response.Content.ReadFromJsonAsync<Dictionary<string, List<string>>>();
        return validationErrors ?? new Dictionary<string, List<string>>();
    }

    public class PaginationMetadata
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}
