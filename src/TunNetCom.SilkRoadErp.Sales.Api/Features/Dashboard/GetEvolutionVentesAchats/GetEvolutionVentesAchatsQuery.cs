using TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Dashboard.GetEvolutionVentesAchats;

public record GetEvolutionVentesAchatsQuery(int Months = 12) : IRequest<EvolutionVentesAchatsResponse>;
