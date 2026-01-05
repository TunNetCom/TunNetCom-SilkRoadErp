namespace TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

public static class PeriodFilterHelper
{
    public static (DateTime? startDate, DateTime? endDate) GetDateRangeFromPeriod(
        string periodFilter,
        DateTime? customStartDate,
        DateTime? customEndDate)
    {
        var today = DateTime.Today;
        var lastMonth = today.AddMonths(-1);

        return periodFilter switch
        {
            "CurrentMonth" => (
                new DateTime(today.Year, today.Month, 1),
                new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59)
            ),
            "LastMonth" => (
                new DateTime(lastMonth.Year, lastMonth.Month, 1),
                new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month), 23, 59, 59)
            ),
            "CurrentYear" => (
                new DateTime(today.Year, 1, 1),
                new DateTime(today.Year, 12, 31, 23, 59, 59)
            ),
            "CustomPeriod" => (
                customStartDate?.Date,
                customEndDate?.Date.AddDays(1).AddTicks(-1)
            ),
            _ => (
                new DateTime(today.Year, today.Month, 1),
                new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59)
            )
        };
    }
}

