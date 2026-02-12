using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;
using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;
using JsonException = System.Text.Json.JsonException;

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
        _ = response.EnsureSuccessStatusCode();
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
                // The API returns the created delivery note Num in the Location header
                var location = response.Headers.Location?.ToString();
                if (int.TryParse(location?.Split('/').Last(), out var deliveryNoteNum))
                {
                    // Convert int to long to match the interface signature
                    return Result.Ok((long)deliveryNoteNum);
                }
                return Result.Fail("Could not extract delivery note number from response");
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
        _ = response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<List<DeliveryNoteResponse>>(content) ?? new List<DeliveryNoteResponse>();
    }

    public async Task<List<DeliveryNoteResponse>> GetDeliveryNotesByInvoiceId(int invoiceId)
    {
        var response = await _httpClient.GetAsync($"/deliveryNote/facture/{invoiceId}");
        _ = response.EnsureSuccessStatusCode();
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

            _ = response.EnsureSuccessStatusCode(); // Ensures 204 No Content

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

            _ = response.EnsureSuccessStatusCode();

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
                int? customerId,
                int? invoiceId,
                bool? isInvoiced,
                string? sortOrder,
                string? sortProperty,
                int pageNumber,
                int pageSize,
                string? searchKeyword,
                DateTime? startDate,
                DateTime? endDate,
                int? status,
                int? technicianId,
                List<int>? tagIds,
                CancellationToken cancellationToken)
    {
        // Validate pagination parameters
        if (pageNumber < 1 || pageSize < 1)
        {
            throw new ArgumentException("PageNumber and PageSize must be greater than zero.");
        }

        // Build query string with only non-null parameters
        var queryParams = new List<string>();

        if (customerId.HasValue)
            queryParams.Add($"customerId={Uri.EscapeDataString(customerId.Value.ToString())}");
        if (invoiceId.HasValue)
            queryParams.Add($"invoiceId={Uri.EscapeDataString(invoiceId.Value.ToString())}");
        if (isInvoiced.HasValue)
            queryParams.Add($"isInvoiced={Uri.EscapeDataString(isInvoiced.Value.ToString())}");
        if (!string.IsNullOrEmpty(sortOrder))
            queryParams.Add($"sortOrder={Uri.EscapeDataString(sortOrder)}");
        if (!string.IsNullOrEmpty(sortProperty))
            queryParams.Add($"sortProperty={Uri.EscapeDataString(sortProperty)}");
        if (!string.IsNullOrEmpty(searchKeyword))
            queryParams.Add($"searchKeyword={Uri.EscapeDataString(searchKeyword)}");
        if (startDate.HasValue)
            queryParams.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("yyyy-MM-dd"))}");
        if (endDate.HasValue)
            queryParams.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("yyyy-MM-dd"))}");
        if (status.HasValue)
            queryParams.Add($"status={Uri.EscapeDataString(status.Value.ToString())}");
        if (technicianId.HasValue)
            queryParams.Add($"technicianId={Uri.EscapeDataString(technicianId.Value.ToString())}");
        if (tagIds != null && tagIds.Any())
        {
            foreach (var tagId in tagIds)
            {
                queryParams.Add($"tagIds={Uri.EscapeDataString(tagId.ToString())}");
            }
        }

        queryParams.Add($"pageNumber={Uri.EscapeDataString(pageNumber.ToString())}");
        queryParams.Add($"pageSize={Uri.EscapeDataString(pageSize.ToString())}");

        // Construct query string
        var queryString = string.Join("&", queryParams);
        var requestUri = $"/deliverynotes/summaries?{queryString}";

        try
        {
            // Make the HTTP GET request
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            _ = response.EnsureSuccessStatusCode();

            // Deserialize the response
            var summariesResponse = await response.Content.ReadFromJsonAsync<GetDeliveryNotesWithSummariesResponse>(
                cancellationToken: cancellationToken);

            return summariesResponse ?? throw new InvalidOperationException("Failed to deserialize the response.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to deserialize delivery notes summaries response.", ex);
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

            _ = response.EnsureSuccessStatusCode();

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

    public async Task<PagedList<DeliveryNoteDetailResponse>> GetDeliveryNotesAsync(
        string productReference,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var queryParams = $"?productReference={Uri.EscapeDataString(productReference)}&PageNumber={pageNumber}&PageSize={pageSize}";
        var response = await _httpClient.GetAsync($"/deliveryNoteHistory{queryParams}", cancellationToken);
        _ = response.EnsureSuccessStatusCode();

        // Deserialize the response
        return await response.Content.ReadFromJsonAsync<PagedList<DeliveryNoteDetailResponse>>(cancellationToken) 
            ?? new PagedList<DeliveryNoteDetailResponse>();
    }

    public async Task<Result> UpdateDeliveryNoteAsync(
        int num,
        CreateDeliveryNoteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"/deliveryNote/{num}",
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result.Ok();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Result.Fail("Delivery note not found");
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

            return Result.Fail($"Failed to update delivery note: {response.StatusCode}");
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

    public async Task<Result> ValidateDeliveryNotesAsync(
        List<int> ids,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new ValidateDeliveryNotesRequest { Ids = ids };
            var response = await _httpClient.PostAsJsonAsync(
                "api/delivery-notes/validate",
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result.Ok();
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

            return Result.Fail($"Failed to validate delivery notes: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<(byte[] Content, string FileName)> ExportDeliveryNotesToPdfAsync(
        DateTime? startDate,
        DateTime? endDate,
        int? customerId,
        int? technicianId,
        List<int>? tagIds,
        int? status,
        string[]? selectedColumns,
        string? orderBy,
        CancellationToken cancellationToken = default)
    {
        var queryString = BuildExportQueryString(startDate, endDate, customerId, technicianId, tagIds, status, selectedColumns, orderBy);
        var requestUri = $"/api/delivery-notes/export/pdf{queryString}";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"BonsLivraison_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return (content, fileName);
    }

    public async Task<(byte[] Content, string FileName)> ExportDeliveryNotesToExcelAsync(
        DateTime? startDate,
        DateTime? endDate,
        int? customerId,
        int? technicianId,
        List<int>? tagIds,
        int? status,
        string[]? selectedColumns,
        string? orderBy,
        CancellationToken cancellationToken = default)
    {
        var queryString = BuildExportQueryString(startDate, endDate, customerId, technicianId, tagIds, status, selectedColumns, orderBy);
        var requestUri = $"/api/delivery-notes/export/excel{queryString}";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"BonsLivraison_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return (content, fileName);
    }

    private static string BuildExportQueryString(
        DateTime? startDate,
        DateTime? endDate,
        int? customerId,
        int? technicianId,
        List<int>? tagIds,
        int? status,
        string[]? selectedColumns,
        string? orderBy)
    {
        var queryParams = new List<string>();
        if (startDate.HasValue)
            queryParams.Add($"startDate={Uri.EscapeDataString(startDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"))}");
        if (endDate.HasValue)
            queryParams.Add($"endDate={Uri.EscapeDataString(endDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"))}");
        if (customerId.HasValue)
            queryParams.Add($"customerId={customerId.Value}");
        if (technicianId.HasValue)
            queryParams.Add($"technicianId={technicianId.Value}");
        if (tagIds != null)
        {
            foreach (var tagId in tagIds)
                queryParams.Add($"tagIds={tagId}");
        }
        if (status.HasValue)
            queryParams.Add($"status={status.Value}");
        if (!string.IsNullOrEmpty(orderBy))
            queryParams.Add($"orderBy={Uri.EscapeDataString(orderBy)}");
        if (selectedColumns != null && selectedColumns.Length > 0)
        {
            foreach (var column in selectedColumns)
                queryParams.Add($"selectedColumns={Uri.EscapeDataString(column)}");
        }
        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }
}
