using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.GetCompteBancaire;

public record GetCompteBancaireQuery(int Id) : IRequest<Result<CompteBancaireResponse>>;
