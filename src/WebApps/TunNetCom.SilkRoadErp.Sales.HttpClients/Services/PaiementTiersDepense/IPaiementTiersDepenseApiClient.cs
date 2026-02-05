using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;
using TunNetCom.SilkRoadErp.Sales.HttpClients;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementTiersDepense;

public interface IPaiementTiersDepenseApiClient
{
    Task<PagedList<PaiementTiersDepenseResponse>> GetPagedAsync(
        int? tiersDepenseFonctionnementId,
        int? accountingYearId,
        DateTime? datePaiementFrom = null,
        DateTime? datePaiementTo = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<OneOf<PaiementTiersDepenseResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<int, BadRequestResponse>> CreateAsync(
        CreatePaiementTiersDepenseRequest request,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        int id,
        UpdatePaiementTiersDepenseRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
}
