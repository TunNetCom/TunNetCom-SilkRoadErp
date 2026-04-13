namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeleteDeliveryNote;

public class DeleteDeliveryNoteCommand : IRequest<Result>
{
    public int Num { get; }

    public DeleteDeliveryNoteCommand(int num)
    {
        Num = num; 
    }

}
