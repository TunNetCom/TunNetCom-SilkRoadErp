namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

public class GetReceiptNoteByIdQuery : IRequest<Result<ReceiptNoteResponse>>
{
    public int Num { get; set; }
    public GetReceiptNoteByIdQuery(int num)
    {
        Num = num;
    }

}

