namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DeleteReceiptNote;

public record DeleteReceiptNoteCommand(int Num) : IRequest<Result>;