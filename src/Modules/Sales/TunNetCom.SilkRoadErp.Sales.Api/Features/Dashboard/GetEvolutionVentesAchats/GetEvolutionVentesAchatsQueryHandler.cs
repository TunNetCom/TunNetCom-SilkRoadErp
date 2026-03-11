using System.Globalization;
using TunNetCom.SilkRoadErp.Sales.Api.Features.Dashboard.GetRecapVentesAchats;
using TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Dashboard.GetEvolutionVentesAchats;

public class GetEvolutionVentesAchatsQueryHandler(IMediator mediator, ILogger<GetEvolutionVentesAchatsQueryHandler> logger)
    : IRequestHandler<GetEvolutionVentesAchatsQuery, EvolutionVentesAchatsResponse>
{
    public async Task<EvolutionVentesAchatsResponse> Handle(GetEvolutionVentesAchatsQuery request, CancellationToken cancellationToken)
    {
        var months = Math.Clamp(request.Months, 1, 24);
        var end = DateTime.UtcNow.Date;
        var start = end.AddMonths(-months + 1);
        start = new DateTime(start.Year, start.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var result = new EvolutionVentesAchatsResponse();

        for (var d = start; d <= end; d = d.AddMonths(1))
        {
            var monthStart = d;
            var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

            var recap = await mediator.Send(new GetRecapVentesAchatsQuery(monthStart, monthEnd.Date), cancellationToken);

            result.Months.Add(new EvolutionMonthDto
            {
                Month = monthStart,
                MonthLabel = monthStart.ToString("MMM yyyy", CultureInfo.InvariantCulture),
                VentesHT = recap.VentesNettes.TotalHT,
                AchatsHT = recap.AchatsNets.TotalHT,
                ResultatHT = recap.Resultat.TotalHT
            });
        }

        logger.LogInformation("Evolution ventes/achats calculated for {Count} months", result.Months.Count);
        return result;
    }
}
