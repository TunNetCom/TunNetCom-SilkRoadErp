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
        var dayOfWeek = (int)today.DayOfWeek; // 0 = Sunday, 1 = Monday, ...
        var mondayOffset = dayOfWeek == 0 ? -6 : 1 - dayOfWeek; // Monday as start of week
        var monday = today.AddDays(mondayOffset);
        var sunday = monday.AddDays(6);

        return periodFilter switch
        {
            "CurrentDay" => (
                today.Date,
                today.Date.AddDays(1).AddTicks(-1)
            ),
            "CurrentWeek" => (
                monday.Date,
                new DateTime(sunday.Year, sunday.Month, sunday.Day, 23, 59, 59)
            ),
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

