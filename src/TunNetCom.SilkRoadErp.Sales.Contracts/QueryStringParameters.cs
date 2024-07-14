namespace TunNetCom.SilkRoadErp.Sales.Contracts;

public class QueryStringParameters
{
    const int maxPageSize = 50;

    public int PageNumber { get; set; } = 1;

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

    private int _pageSize = 10;
}
