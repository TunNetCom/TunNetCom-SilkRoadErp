using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.GetDeliveryNotesBaseInfosWithSummaries;

public record GetDeliveryNotesBaseInfosWithSummariesQuery(
    int PageNumber,
    int PageSize,
    bool? IsInvoiced,
    int? CustomerId,
    int? InvoiceId,
    string? SortOrder,
    string? SortProperty,
    string? SearchKeyword,
    DateTime? StartDate,
    DateTime? EndDate
    ) : IRequest<GetDeliveryNotesWithSummariesResponse>;