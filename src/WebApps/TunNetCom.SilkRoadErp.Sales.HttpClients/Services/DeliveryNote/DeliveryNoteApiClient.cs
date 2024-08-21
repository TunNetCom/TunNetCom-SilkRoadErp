using Newtonsoft.Json;
using System.Net.Http.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;

public class DeliveryNoteApiClient(HttpClient _httpClient) : IDeliveryNoteApiClient
{
    public async Task<PagedList<DeliveryNoteResponse>> GetDeliveryNotes(int pageNumber, int pageSize, int? customerId, string? searchKeyword, bool? isFactured)
    {
        var queryString = $"/deliveryNote?pageNumber={pageNumber}&pageSize={pageSize}";

        if (customerId.HasValue)
        {
            queryString += $"&customerId={customerId.Value}";
        }

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            queryString += $"&searchKeyword={Uri.EscapeDataString(searchKeyword)}";
        }

        if (isFactured.HasValue)
        {
            queryString += $"&isFactured={isFactured.Value}";
        }

        var response = await _httpClient.GetAsync(queryString);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<PagedList<DeliveryNoteResponse>>();
        return content!;
    }




    public async Task<int> CreateDeliveryNote(CreateDeliveryNoteRequest createDeliveryNoteRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("/deliveryNote", createDeliveryNoteRequest);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Response JSON: " + responseJson);


        var deliveryNoteResponse = JsonConvert.DeserializeObject<DeliveryNoteResponse>(responseJson);
        return deliveryNoteResponse.Num;
    }

    public async Task<List<DeliveryNoteResponse>> GetDeliveryNotesByClientId(int clientId)
    {
        var response = await _httpClient.GetAsync($"/deliveryNote/client/{clientId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<List<DeliveryNoteResponse>>(content) ?? new List<DeliveryNoteResponse>();
    }



}
