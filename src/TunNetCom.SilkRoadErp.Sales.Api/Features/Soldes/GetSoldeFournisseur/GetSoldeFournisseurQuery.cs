using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Soldes.GetSoldeFournisseur;

public record GetSoldeFournisseurQuery(int FournisseurId, int? AccountingYearId = null) : IRequest<Result<SoldeFournisseurResponse>>;

