namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DeleteReceiptNote;

public record DeleteReceiptNoteCommand : IRequest<Result>
{
    public int Num { get; }

    public DeleteReceiptNoteCommand(int num)
    {
        Num = num;
    }
}
