using TunNetCom.SilkRoadErp.Sales.Contracts.FactureAvoirFournisseur;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.GetFullFactureAvoirFournisseur;

public record GetFullFactureAvoirFournisseurQuery(int Id) : IRequest<Result<FullFactureAvoirFournisseurResponse>>;

