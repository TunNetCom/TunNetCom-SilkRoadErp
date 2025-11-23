using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetAvoirFournisseur;

public record GetAvoirFournisseurQuery(int Num) : IRequest<Result<AvoirFournisseurResponse>>;

