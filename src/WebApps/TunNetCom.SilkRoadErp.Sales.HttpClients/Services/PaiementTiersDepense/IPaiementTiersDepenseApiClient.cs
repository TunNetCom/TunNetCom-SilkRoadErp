using TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;
using TunNetCom.SilkRoadErp.Sales.HttpClients;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.PaiementTiersDepense;

public interface IPaiementTiersDepenseApiClient
{
    Task<PagedList<PaiementTiersDepenseResponse>> GetPagedAsync(
        int? tiersDepenseFonctionnementId,
        int? accountingYearId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

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
}
