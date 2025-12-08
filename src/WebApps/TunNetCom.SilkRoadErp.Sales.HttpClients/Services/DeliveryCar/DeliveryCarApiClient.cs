using System.Net.Http.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryCar;

public class DeliveryCarApiClient : IDeliveryCarApiClient
{
    private readonly HttpClient _httpClient;

    public DeliveryCarApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DeliveryCarResponse>> GetDeliveryCarsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<List<DeliveryCarResponse>>(
            "/delivery-cars",
            cancellationToken);
        return response ?? new List<DeliveryCarResponse>();
    }

    public async Task<DeliveryCarResponse?> GetDeliveryCarByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<DeliveryCarResponse>(
            $"/delivery-cars/{id}",
            cancellationToken);
    }

    public async Task<int> CreateDeliveryCarAsync(CreateDeliveryCarRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/delivery-cars",
            request,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var locationHeader = response.Headers.Location?.ToString();
        if (locationHeader != null)
        {
            var id = int.Parse(locationHeader.Split('/').Last());
            return id;
        }
        return 0;
    }

    public async Task UpdateDeliveryCarAsync(int id, UpdateDeliveryCarRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"/delivery-cars/{id}",
            request,
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteDeliveryCarAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"/delivery-cars/{id}",
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}



