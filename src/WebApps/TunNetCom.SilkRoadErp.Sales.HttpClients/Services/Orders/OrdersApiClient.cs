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
    }
}