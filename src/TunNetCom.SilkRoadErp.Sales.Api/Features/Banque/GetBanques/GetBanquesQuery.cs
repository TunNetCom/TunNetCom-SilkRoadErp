using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Banque.GetBanques;

public record GetBanquesQuery() : IRequest<Result<List<BanqueResponse>>>;

