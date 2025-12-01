using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductFamilies.GetFamilleProduits;

public record GetFamilleProduitsQuery() : IRequest<List<FamilleProduitResponse>>;

