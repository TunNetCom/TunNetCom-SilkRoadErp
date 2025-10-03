using System.Reflection;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Request;
public class GetReceiptNoteLinesWithSummariesQueryParams
{
    public string? SortOrder { get; set; }
    public string? SortProperty { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchKeyword { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string GetQuery()
    {
        var parts = GetType()
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Select(p => new { p.Name, Value = p.GetValue(this) })
        .Where(x =>
            x.Value != null &&
            (!(x.Value is string s) || !string.IsNullOrWhiteSpace(s)))
        .Select(x =>
        {
            string valueStr = x.Value is DateTime dt
                ? dt.ToString("o")
                : x.Value.ToString()!;
            return $"{x.Name}={Uri.EscapeDataString(valueStr)}";
        });

        var query = string.Join("&", parts);
        return string.IsNullOrEmpty(query) ? "" : "?" + query;
    }
}
