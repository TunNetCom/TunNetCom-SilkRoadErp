namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByNum;

public class GetDeliveryNoteByNumQuery : IRequest<Result<DeliveryNoteResponse>>
{
    public int Num { get; set; }

    public GetDeliveryNoteByNumQuery(int num)
    {
        Num = num;
    }
}
