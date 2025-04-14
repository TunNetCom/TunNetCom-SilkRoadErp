using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;

public class DeliveryNoteApiClient(HttpClient _httpClient) : IDeliveryNoteApiClient
{
    public async Task<PagedList<DeliveryNoteResponse>> GetDeliveryNotes(
        int pageNumber,
        int pageSize,
        int? customerId,
        string? searchKeyword,
        bool? isFactured)
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

    public async Task<Result<long>> CreateDeliveryNoteAsync(
       CreateDeliveryNoteRequest request,
       CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/deliveryNote",
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                // Assuming the API returns the created delivery note ID in the Location header
                var location = response.Headers.Location?.ToString();
                if (long.TryParse(location?.Split('/').Last(), out var deliveryNoteId))
                {
                    return Result.Ok(deliveryNoteId);
                }
                return Result.Fail("Could not extract delivery note ID from response");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var problemDetails = await response.Content.ReadFromJsonAsync<BadRequestResponse>(
                    cancellationToken);

                if (problemDetails?.errors != null)
                {
                    var errors = problemDetails.errors
                        .SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}: {v}"));
                    return Result.Fail(errors);
                }
                return Result.Fail("Validation failed but no error details provided");
            }

            return Result.Fail($"Failed to create delivery note: {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            return Result.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
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

    public async Task<GetDeliveryNotesWithSummariesResponse> GetDeliveryNotesWithSummariesAsync(
        int customerId,
        int? invoiceId,
        bool isInvoiced,
        string? sortOrder,
        string? sortProperty,
        CancellationToken cancellationToken,
        int pageNumber = 1,
        int pageSize = 10)
    {
        // Construct the query string with all parameters
        var queryParams = new List<string>
        {
            $"customerId={customerId}",
            $"isInvoiced={isInvoiced}",
            $"pageNumber={pageNumber}",
            $"pageSize={pageSize}",
            $"sortOrder={sortOrder}",
            $"sortProperty={sortProperty}"
        };
        var GetDeliveryNotesWithSummariesQueryParams = new GetDeliveryNotesQueryParams
        {
            CustomerId = customerId,
            IsInvoiced = isInvoiced,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortOrder = sortOrder,
            SortProperty = sortProperty,
            InvoiceId = invoiceId
        };
        if (invoiceId.HasValue)
        {
            queryParams.Add($"invoiceId={invoiceId.Value}");
        }

        var queryString = $"/deliverynotes/summaries?{string.Join("&", queryParams)}";

        try
        {
            // Make the HTTP GET request
            var response = await _httpClient.GetAsync(queryString, cancellationToken);
            response.EnsureSuccessStatusCode();

            // Deserialize the response into GetDeliveryNotesWithSummariesResponse
            var summariesResponse = await response.Content.ReadFromJsonAsync<GetDeliveryNotesWithSummariesResponse>(cancellationToken: cancellationToken);

            return summariesResponse ?? throw new InvalidOperationException("Failed to deserialize the response.");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching delivery notes summaries: {ex.Message}");
            throw;
        }
        catch (System.Text.Json.JsonException ex)
        {
            Console.WriteLine($"Error deserializing response: {ex.Message}");
            throw;
        }
    }

    public async Task<DeliveryNoteResponse?> GetDeliveryNoteByNumAsync(
        int num,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/deliveryNote/{num}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var deliveryNote = await response
                .Content
                .ReadFromJsonAsync<DeliveryNoteResponse>(cancellationToken: cancellationToken);

            if (deliveryNote is null)
            {
                throw new InvalidOperationException("Failed to deserialize the delivery note response.");
            }

            return deliveryNote;
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to fetch delivery note: {ex.Message}", ex);
        }
    }
}
