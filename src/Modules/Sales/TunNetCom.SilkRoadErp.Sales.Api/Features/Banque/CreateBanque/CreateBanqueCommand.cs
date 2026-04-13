using TunNetCom.SilkRoadErp.Sales.Contracts.Banque;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Banque.CreateBanque;

public record CreateBanqueCommand(string Nom) : IRequest<Result<int>>;

