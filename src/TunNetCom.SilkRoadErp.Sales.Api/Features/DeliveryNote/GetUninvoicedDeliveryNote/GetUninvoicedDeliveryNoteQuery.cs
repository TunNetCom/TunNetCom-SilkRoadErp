using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetUninvoicedDeliveryNote;
public class GetUninvoicedDeliveryNoteQuery : IRequest<Result<List<DeliveryNoteResponse>>>
{
    public int ClientId { get; set; }

    public GetUninvoicedDeliveryNoteQuery(int clientId)
    {
        ClientId = clientId;
    }
}
