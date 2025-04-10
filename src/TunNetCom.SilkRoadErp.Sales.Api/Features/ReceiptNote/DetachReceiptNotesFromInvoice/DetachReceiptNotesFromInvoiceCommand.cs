namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.DetachReceiptNotesFromInvoice;

public record DetachReceiptNotesFromInvoiceCommand(int InvoiceId, List<int> ReceiptNoteIds) : IRequest<Result>;
