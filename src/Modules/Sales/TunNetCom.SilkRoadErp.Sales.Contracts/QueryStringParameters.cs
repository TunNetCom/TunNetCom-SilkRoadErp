namespace TunNetCom.SilkRoadErp.Sales.Contracts;

public class QueryStringParameters
{
    const int maxPageSize = 50;

    public int PageNumber { get; set; } = 1;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? SearchKeyword { get; set; }

    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }

    public string? SortProprety { get; set; }

    public string? SortOrder { get; set; }

    public int? FamilleProduitId { get; set; }

    public int? SousFamilleProduitId { get; set; }

    public bool? Visibility { get; set; }

    public bool? StockLowOnly { get; set; }

    public int? StockCalculeMin { get; set; }

    public int? StockCalculeMax { get; set; }

    private int _pageSize = 10;
}
