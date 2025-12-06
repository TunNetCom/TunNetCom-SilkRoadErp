using System.Net;
using TunNetCom.SilkRoadErp.Sales.Contracts.Common;
using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;
using JsonException = System.Text.Json.JsonException;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Quotations;

public class QuotationApiClient(HttpClient httpClient, ILogger<QuotationApiClient> logger) : IQuotationApiClient
{
    public async Task<PagedList<QuotationResponse>> GetQuotationsAsync(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken)
    {
        try
        {
            var queryString = $"/quotations?pageNumber={queryParameters.PageNumber}&pageSize={queryParameters.PageSize}";

            if (!string.IsNullOrEmpty(queryParameters.SearchKeyword))
            {
                queryString += $"&searchKeyword={Uri.EscapeDataString(queryParameters.SearchKeyword)}";
            }

            if (!string.IsNullOrEmpty(queryParameters.SortOrder))
            {
                queryString += $"&sortOrder={Uri.EscapeDataString(queryParameters.SortOrder)}";
            }

            if (!string.IsNullOrEmpty(queryParameters.SortProprety))
            {
                queryString += $"&sortProperty={Uri.EscapeDataString(queryParameters.SortProprety)}";
            }

            var response = await httpClient.GetAsync(queryString, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<PagedList<QuotationResponse>>(cancellationToken: cancellationToken);
            return content ?? new PagedList<QuotationResponse>();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to fetch quotations");
            throw;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize quotations response");
            throw;
        }
    }

    public async Task<Result<FullQuotationResponse>> GetQuotationByIdAsync(
        int num,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.GetAsync($"/quotations/{num}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.Fail("Quotation not found");
            }

            response.EnsureSuccessStatusCode();

            var quotation = await response.Content.ReadFromJsonAsync<FullQuotationResponse>(cancellationToken: cancellationToken);

            if (quotation is null)
            {
                return Result.Fail("Failed to deserialize quotation response");
            }

            return Result.Ok(quotation);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to fetch quotation {Num}", num);
            return Result.Fail($"Network error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize quotation {Num}", num);
            return Result.Fail($"Deserialization error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error fetching quotation {Num}", num);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateQuotationAsync(
        CreateQuotationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                "/quotations",
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var location = response.Headers.Location?.ToString();
                if (location != null && int.TryParse(location.Split('/').Last(), out var quotationNum))
                {
                    return Result.Ok(quotationNum);
                }

                // Try to read the response body if location header is not available
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                if (int.TryParse(responseContent, out var numFromBody))
                {
                    return Result.Ok(numFromBody);
                }

                return Result.Fail("Could not extract quotation number from response");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var problemDetails = await response.Content.ReadFromJsonAsync<BadRequestResponse>(
                    cancellationToken: cancellationToken);

                if (problemDetails?.errors != null)
                {
                    var errors = problemDetails.errors
                        .SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}: {v}"));
                    return Result.Fail(errors);
                }
                return Result.Fail("Validation failed but no error details provided");
            }

            return Result.Fail($"Failed to create quotation: {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error creating quotation");
            return Result.Fail($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating quotation");
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result> UpdateQuotationAsync(
        int num,
        UpdateQuotationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync(
                $"/quotations/{num}",
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result.Ok();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.Fail("Quotation not found");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var problemDetails = await response.Content.ReadFromJsonAsync<BadRequestResponse>(
                    cancellationToken: cancellationToken);

                if (problemDetails?.errors != null)
                {
                    var errors = problemDetails.errors
                        .SelectMany(kvp => kvp.Value.Select(v => $"{kvp.Key}: {v}"));
                    return Result.Fail(errors);
                }
                return Result.Fail("Validation failed but no error details provided");
            }

            return Result.Fail($"Failed to update quotation: {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error updating quotation {Num}", num);
            return Result.Fail($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error updating quotation {Num}", num);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result> DeleteQuotationAsync(
        int num,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"/quotations/{num}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result.Ok();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Result.Fail("Quotation not found");
            }

            return Result.Fail($"Failed to delete quotation: {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Network error deleting quotation {Num}", num);
            return Result.Fail($"Network error: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error deleting quotation {Num}", num);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<Result> ValidateQuotationsAsync(List<int> ids, CancellationToken cancellationToken)
    {
        logger.LogInformation("Validating quotations via API api/quotations/validate");
        var response = await httpClient.PostAsJsonAsync("api/quotations/validate", ids, cancellationToken: cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return Result.Fail("quotations_not_found");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequest = await response.ReadJsonAsync<BadRequestResponse>();
            return Result.Fail(badRequest.Detail ?? badRequest.Title ?? "Unknown error");
        }

        throw new Exception($"Quotations validation: Unexpected response. Status Code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
    }
}

