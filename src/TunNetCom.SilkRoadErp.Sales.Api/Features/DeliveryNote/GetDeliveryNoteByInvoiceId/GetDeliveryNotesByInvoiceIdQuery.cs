namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNoteByInvoiceId;

public record GetDeliveryNotesByInvoiceIdQuery (int NumFacture) : IRequest<Result<List<DeliveryNoteResponse>>>;
