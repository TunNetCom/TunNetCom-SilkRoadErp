using TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProductSubFamilies.GetSousFamilleProduits;

public record GetSousFamilleProduitsQuery(int? FamilleProduitId = null) : IRequest<List<SousFamilleProduitResponse>>;

