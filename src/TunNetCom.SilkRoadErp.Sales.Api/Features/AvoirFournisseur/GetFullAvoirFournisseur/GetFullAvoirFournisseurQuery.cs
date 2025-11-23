using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetFullAvoirFournisseur;

public record GetFullAvoirFournisseurQuery(int Num) : IRequest<Result<FullAvoirFournisseurResponse>>;

