using System.Net;
using TunNetCom.SilkRoadErp.Sales.Contracts.Common;
using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Orders
{
    public class OrderApiClient : IOrderApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderApiClient> _logger;

        public OrderApiClient(HttpClient httpClient, ILogger<OrderApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<FullOrderResponse> GetFullOrderAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching full order with ID {OrderId}", id);

            var response = await _httpClient.GetAsync($"api/orders/{id}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return new FullOrderResponse();
            }

            _ = response.EnsureSuccessStatusCode();
            
            var order = await response.Content.ReadFromJsonAsync<FullOrderResponse>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            _logger.LogInformation("Successfully retrieved order with ID {OrderId}", id);
            return order ?? new FullOrderResponse();
        }

        public async Task<List<OrderSummaryResponse>> GetOrdersListAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching orders list");

            var response = await _httpClient.GetAsync("api/orders", cancellationToken);

            _ = response.EnsureSuccessStatusCode();
            
            var orders = await response.Content.ReadFromJsonAsync<List<OrderSummaryResponse>>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            if (orders is null)
            {
                _logger.LogWarning("Orders list is empty");
                orders = new List<OrderSummaryResponse>(); // Return empty list instead of null
            }

            _logger.LogInformation("Successfully retrieved {Count} orders", orders.Count);
            return orders ?? new List<OrderSummaryResponse>();
        }

        public async Task<Result<int>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Creating order via API /orders");

                var response = await _httpClient.PostAsJsonAsync("/orders", request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var location = response.Headers.Location?.ToString();
                    if (location != null && int.TryParse(location.Split('/').Last(), out var orderNum))
                    {
                        return Result.Ok(orderNum);
                    }

                    // Try to read the response body if location header is not available
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    if (int.TryParse(responseContent, out var numFromBody))
                    {
                        return Result.Ok(numFromBody);
                    }

                    return Result.Fail("Could not extract order number from response");
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

                return Result.Fail($"Failed to create order: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error creating order");
                return Result.Fail($"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating order");
                return Result.Fail($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<Result> UpdateOrderAsync(int num, UpdateOrderRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Updating order via API /orders/{Num}", num);

                var response = await _httpClient.PutAsJsonAsync($"/orders/{num}", request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return Result.Ok();
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Result.Fail("Order not found");
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

                return Result.Fail($"Failed to update order: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error updating order {Num}", num);
                return Result.Fail($"Network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating order {Num}", num);
                return Result.Fail($"Unexpected error: {ex.Message}");
            }
        }
    }
}