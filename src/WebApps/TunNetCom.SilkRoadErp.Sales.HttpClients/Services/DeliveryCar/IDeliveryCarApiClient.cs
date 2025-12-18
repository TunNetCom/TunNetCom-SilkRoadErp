using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryCar;

public interface IDeliveryCarApiClient
{
    Task<List<DeliveryCarResponse>> GetDeliveryCarsAsync(CancellationToken cancellationToken = default);
    Task<DeliveryCarResponse?> GetDeliveryCarByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateDeliveryCarAsync(CreateDeliveryCarRequest request, CancellationToken cancellationToken = default);
    Task UpdateDeliveryCarAsync(int id, UpdateDeliveryCarRequest request, CancellationToken cancellationToken = default);
    Task DeleteDeliveryCarAsync(int id, CancellationToken cancellationToken = default);
}





