using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.GetReceiptNoteWithDetails;

public record GetReceiptNoteWithDetailsQuery(
    QueryStringParameters queryStringParameters,
    int IdFournisseur,
    bool IsInvoiced,
    int? InvoiceId) : IRequest<ReceiptNotesWithSummary>;
