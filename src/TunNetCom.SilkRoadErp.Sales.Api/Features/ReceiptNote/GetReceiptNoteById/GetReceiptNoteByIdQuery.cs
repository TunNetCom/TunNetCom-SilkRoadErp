namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteById;

public record GetReceiptNoteByIdQuery(int Num) : IRequest<Result<ReceiptNoteResponse>>;

