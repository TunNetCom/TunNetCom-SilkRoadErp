using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.GetTiersDepenseFonctionnement;

public record GetTiersDepenseFonctionnementQuery(
    int PageNumber,
    int PageSize,
    string? SearchKeyword) : IRequest<PagedList<TiersDepenseFonctionnementResponse>>;
