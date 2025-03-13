namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.AttachToInvoice;

public record AttachToInvoiceCommand(int InvoiceId, int DeliveryNoteId): IRequest<Result>;
