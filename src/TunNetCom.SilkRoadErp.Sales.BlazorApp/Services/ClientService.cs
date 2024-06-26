using Serilog;
using System.Net.Http.Json;
using System.Net;
using TunNetCom.SilkRoadErp.Sales.Api.Contracts.Client;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Clients.GetClient;

namespace TunNetCom.SilkRoadErp.Sales.BlazorApp.Services;

public class ClientService
{
    private readonly HttpClient _httpClient;
    private readonly Serilog.ILogger _logger;

    public ClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _logger = Log.ForContext<ClientService>();
    }

    public async Task<(List<ClientResponse> Clients, int TotalCount)> GetClientsAsync(int pageIndex, int pageSize)
    {
        _logger.Information("Fetching clients with pageIndex: {PageIndex} and pageSize: {PageSize}", pageIndex, pageSize);

        var response = await _httpClient.GetFromJsonAsync<PaginatedResponse<ClientResponse>>($"/clients?pageIndex={pageIndex}&pageSize={pageSize}");

        _logger.Information("Fetched {Count} clients", response.Items.Count);

        return (response.Items, response.TotalCount);
    }

    public async Task<ClientResponse> GetClientByIdAsync(int id)
    {
        _logger.Information("Fetching client with ID: {Id}", id);

        var client = await _httpClient.GetFromJsonAsync<ClientResponse>($"/clients/{id}");

        if (client == null)
        {
            _logger.Warning("Client with ID: {Id} not found", id);
        }
        else
        {
            _logger.Information("Fetched client with ID: {Id}", id);
        }

        return client;
    }

    public async Task AddClientAsync(CreateClientRequest client)
    {
        _logger.Information("Adding new client");

        var response = await _httpClient.PostAsJsonAsync("/clients", client);
        await HandleResponse(response);

        _logger.Information("Added new client");
    }

    public async Task UpdateClientAsync(int id, UpdateClientRequest client)
    {
        _logger.Information("Updating client with ID: {Id}", id);

        var response = await _httpClient.PutAsJsonAsync($"/clients/{id}", client);
        await HandleResponse(response);

        _logger.Information("Updated client with ID: {Id}", id);
    }

    public async Task DeleteClientAsync(int id)
    {
        _logger.Information("Deleting client with ID: {Id}", id);

        var response = await _httpClient.DeleteAsync($"/clients/{id}");
        await HandleResponse(response);

        _logger.Information("Deleted client with ID: {Id}", id);
    }

    private async Task HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.Error("HTTP request failed with status code {StatusCode} and content: {ErrorContent}", response.StatusCode, errorContent);
            throw new ClientServiceException(response.StatusCode, errorContent);
        }
    }


}
