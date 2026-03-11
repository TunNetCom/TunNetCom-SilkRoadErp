using TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.GetAvoirFournisseur;

public record GetAvoirFournisseurQuery(int Id) : IRequest<Result<AvoirFournisseurResponse>>;

