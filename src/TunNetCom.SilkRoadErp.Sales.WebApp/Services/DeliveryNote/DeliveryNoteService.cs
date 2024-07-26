using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.DeliveryNote;

public class DeliveryNoteService(HttpClient _httpClient)
{
    public async Task<PagedList<DeliveryNoteResponse>> GetDeliveryNotes(int pageNumber, int pageSize, string? searchKeyword, bool? isFactured)
    {
        var response = await _httpClient.GetAsync($"/deliveryNotes?pageNumber={pageNumber}&pageSize={pageSize}&searchKeyword={searchKeyword}&isFactured={isFactured}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<PagedList<DeliveryNoteResponse>>();
        return content!;
    }

    public async Task<int> CreateDeliveryNote(CreateDeliveryNoteRequest createDeliveryNoteRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/deliveryNote", createDeliveryNoteRequest);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }
}
