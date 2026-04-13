namespace TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

/// <summary>
/// API response for GET /api/dashboard/evolution. One point per month for charting.
/// </summary>
public class EvolutionVentesAchatsResponse
{
    public List<EvolutionMonthDto> Months { get; set; } = new();
}

public class EvolutionMonthDto
{
    /// <summary>First day of the month (e.g. 2025-01-01) for ordering and display.</summary>
    public DateTime Month { get; set; }

    /// <summary>Label for chart category (e.g. "Janv. 2025").</summary>
    public string MonthLabel { get; set; } = string.Empty;

    public decimal VentesHT { get; set; }
    public decimal AchatsHT { get; set; }
    public decimal ResultatHT { get; set; }
}
