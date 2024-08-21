using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.DeliveryNote;

public interface IDeliveryNoteApiClient
{
    Task<PagedList<DeliveryNoteResponse>> GetDeliveryNotes(
        int pageNumber,
        int pageSize,
        int? customerId,
        string? searchKeyword,
        bool? isFactured);

    Task<int> CreateDeliveryNote(
        CreateDeliveryNoteRequest createDeliveryNoteRequest);

    Task<List<DeliveryNoteResponse>> GetDeliveryNotesByClientId(
        int clientId);
}
