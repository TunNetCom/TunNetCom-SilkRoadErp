using TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;
using TunNetCom.SilkRoadErp.Sales.HttpClients;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.FactureDepense;

public interface IFactureDepenseApiClient
{
    Task<GetFacturesDepenseWithSummariesResponse> GetWithSummariesAsync(
        int pageNumber,
        int pageSize,
        int? tiersDepenseFonctionnementId,
        int? accountingYearId,
        string? searchKeyword,
        CancellationToken cancellationToken);

    Task<OneOf<FactureDepenseResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<int, BadRequestResponse>> CreateAsync(
        CreateFactureDepenseRequest request,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        int id,
        UpdateFactureDepenseRequest request,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, BadRequestResponse>> ValidateAsync(
        int id,
        CancellationToken cancellationToken);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
}
