using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TunNetCom.SilkRoadErp.Sales.Contracts.Customers;
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

    public async Task<List<DeliveryNoteResponse>> GetDeliveryNotesByInvoiceId(int invoiceId)
    {
        var response = await _httpClient.GetAsync($"/deliveryNote/facture/{invoiceId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<List<DeliveryNoteResponse>>(content) ?? new List<DeliveryNoteResponse>();
    }
    

    public async Task<bool> AttachToInvoiceAsync(AttachToInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            string endpoint = "/deliveryNote/attachToInvoice";
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception($"Invoice or delivery notes not found. InvoiceId: {request.InvoiceId}, DeliveryNoteIds: {string.Join(", ", request.DeliveryNoteIds)}");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string errorResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Validation error: {errorResponse}");
            }

            response.EnsureSuccessStatusCode(); // Ensures 204 No Content

            return true; // Success
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to attach delivery notes to invoice", ex);
        }
    }
    public async Task<List<DeliveryNoteResponse>> GetUninvoicedDeliveryNotesAsync(int clientId, CancellationToken cancellationToken)
    {
        try
        {
            string endpoint = $"/deliveryNote/uninvoiced/{clientId}";
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<DeliveryNoteResponse>(); // Return an empty list if not found
            }

            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            var deliveryNotes = System.Text.Json.JsonSerializer.Deserialize<List<DeliveryNoteResponse>>(jsonResponse);

            return deliveryNotes ?? new List<DeliveryNoteResponse>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve uninvoiced delivery notes for client {clientId}", ex);
        }
    }

    public async Task<OneOf<bool, BadRequestResponse>> DetachFromInvoiceAsync(
DetachFromInvoiceRequest request,
CancellationToken cancellationToken)
    {
        var headers = new Dictionary<string, string>()
    {
        { "Accept", "application/problem+json" }
    };

        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(
            "/deliveryNote/detachFromInvoice",
            request,
            headers,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new Exception($"Invoice {request.InvoiceId} or delivery notes not found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return await response.ReadJsonAsync<BadRequestResponse>();
        }

        throw new Exception($"Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }

}