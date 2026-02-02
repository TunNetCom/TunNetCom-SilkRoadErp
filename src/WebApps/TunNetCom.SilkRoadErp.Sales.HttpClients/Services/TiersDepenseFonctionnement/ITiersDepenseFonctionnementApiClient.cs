using TunNetCom.SilkRoadErp.Sales.Contracts;
using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;
using TunNetCom.SilkRoadErp.Sales.HttpClients;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.TiersDepenseFonctionnement;

public interface ITiersDepenseFonctionnementApiClient
{
    Task<PagedList<TiersDepenseFonctionnementResponse>> GetPagedAsync(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);

    Task<OneOf<TiersDepenseFonctionnementResponse, bool>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<int, BadRequestResponse>> CreateAsync(
        CreateTiersDepenseFonctionnementRequest request,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        int id,
        UpdateTiersDepenseFonctionnementRequest request,
        CancellationToken cancellationToken);
}
