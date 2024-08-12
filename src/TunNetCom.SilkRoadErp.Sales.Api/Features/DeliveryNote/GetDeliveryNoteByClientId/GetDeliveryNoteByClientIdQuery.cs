namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByClientId;

public class GetDeliveryNoteByClientIdQuery : IRequest<Result<List<DeliveryNoteResponse>>>
{
    public int ClientId { get; set; }

    public GetDeliveryNoteByClientIdQuery(int clientId)
    {
        ClientId = clientId;
    }
}
