using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFactureAvoirFournisseur;

public record GetFactureAvoirFournisseurQuery(int Num) : IRequest<Result<FactureAvoirFournisseurResponse>>;

