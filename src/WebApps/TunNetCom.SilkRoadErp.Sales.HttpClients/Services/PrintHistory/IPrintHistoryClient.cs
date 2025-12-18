using TunNetCom.SilkRoadErp.Sales.Contracts.PrintHistory;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PrintHistory;

public interface IPrintHistoryClient
{
    Task<PagedList<PrintHistoryResponse>> GetPrintHistoryAsync(
        GetPrintHistoryRequest request,
        CancellationToken cancellationToken);

    Task<List<PrintHistoryResponse>> GetPrintHistoryByDocumentAsync(
        string documentType,
        int documentId,
        CancellationToken cancellationToken);

    Task<long> CreatePrintHistoryAsync(
        CreatePrintHistoryRequest request,
        CancellationToken cancellationToken);
}








