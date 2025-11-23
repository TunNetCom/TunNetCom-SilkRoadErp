using TunNetCom.SilkRoadErp.Sales.Contracts.Common;
using TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Quotations;

public interface IQuotationApiClient
{
    Task<PagedList<QuotationResponse>> GetQuotationsAsync(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);

    Task<Result<FullQuotationResponse>> GetQuotationByIdAsync(
        int num,
        CancellationToken cancellationToken);

    Task<Result<int>> CreateQuotationAsync(
        CreateQuotationRequest request,
        CancellationToken cancellationToken);

    Task<Result> UpdateQuotationAsync(
        int num,
        UpdateQuotationRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeleteQuotationAsync(
        int num,
        CancellationToken cancellationToken);
}

