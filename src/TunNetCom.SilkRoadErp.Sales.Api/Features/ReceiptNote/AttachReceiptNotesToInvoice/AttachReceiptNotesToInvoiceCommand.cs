namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.AttachReceiptNotesToInvoice;

public record AttachReceiptNotesToInvoiceCommand (List<int> ReceiptNotesIds, int InvoiceId) : IRequest<Result>;