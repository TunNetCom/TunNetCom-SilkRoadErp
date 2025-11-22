using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Orders;
 public interface IOrderApiClient
    {
        /// <summary>
        /// Retrieves a full order by its ID, including supplier details and order lines.
        /// </summary>
        /// <param name="id">The ID of the order (Num).</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>A Result containing the FullOrderResponse or an error.</returns>
        Task<FullOrderResponse> GetFullOrderAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a list of orders with summary information (ID, supplier ID, date, totals).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>A Result containing a list of OrderSummaryResponse or an error.</returns>
        Task<List<OrderSummaryResponse>> GetOrdersListAsync(CancellationToken cancellationToken = default);
    }