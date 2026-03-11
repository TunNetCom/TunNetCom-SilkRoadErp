namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DetachFromInvoice;

public record DetachFromInvoiceCommand (int InvoiceId , List<int> DeliveryNoteIds) : IRequest<Result>;

