using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PrintHistory.GetPrintHistoryByDocument;

public record GetPrintHistoryByDocumentQuery(
    string DocumentType,
    int DocumentId) : IRequest<List<PrintHistoryResponse>>;

