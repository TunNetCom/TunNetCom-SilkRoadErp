using TunNetCom.SilkRoadErp.Sales.Contracts.CompteBancaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.CompteBancaire.GetCompteBancaires;

public record GetCompteBancairesQuery() : IRequest<Result<List<CompteBancaireResponse>>>;
