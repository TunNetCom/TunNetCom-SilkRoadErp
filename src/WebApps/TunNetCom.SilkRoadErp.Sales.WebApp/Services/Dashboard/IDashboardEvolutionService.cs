using TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Dashboard;

public interface IDashboardEvolutionService
{
    Task<EvolutionVentesAchatsResponse?> GetEvolutionAsync(int months = 12, CancellationToken cancellationToken = default);
}
