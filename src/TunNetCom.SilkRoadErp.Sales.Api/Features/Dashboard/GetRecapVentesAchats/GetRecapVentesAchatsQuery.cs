using TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Dashboard.GetRecapVentesAchats;

public record GetRecapVentesAchatsQuery(DateTime StartDate, DateTime EndDate) : IRequest<RecapVentesAchatsResponse>;
