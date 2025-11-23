using MediatR;
using TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.GetInvoicesWithSummaries;

public record GetInvoicesWithSummariesQuery(
    int PageNumber,
    int PageSize,
    int? CustomerId,
    string? SortOrder,
    string? SortProperty,
    string? SearchKeyword,
    DateTime? StartDate,
    DateTime? EndDate
) : IRequest<GetInvoicesWithSummariesResponse>;

