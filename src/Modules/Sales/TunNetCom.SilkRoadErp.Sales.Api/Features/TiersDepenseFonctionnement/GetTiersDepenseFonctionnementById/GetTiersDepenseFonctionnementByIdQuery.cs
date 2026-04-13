using TunNetCom.SilkRoadErp.Sales.Contracts.TiersDepenseFonctionnement;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.TiersDepenseFonctionnement.GetTiersDepenseFonctionnementById;

public record GetTiersDepenseFonctionnementByIdQuery(int Id) : IRequest<Result<TiersDepenseFonctionnementResponse>>;
