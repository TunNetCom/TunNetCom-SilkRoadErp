namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.AttachToInvoice;

public record AttachToInvoiceCommand(int InvoiceId, List<int> DeliveryNoteIds) : IRequest<Result>;
