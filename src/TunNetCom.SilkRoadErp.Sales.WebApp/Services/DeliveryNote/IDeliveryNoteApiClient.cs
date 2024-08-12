namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.DeliveryNote;

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
