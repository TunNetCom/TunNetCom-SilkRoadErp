using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

public class GetInventairesQueryParams
{
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; } = 1;

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; } = 10;

    [JsonPropertyName("searchKeyword")]
    public string? SearchKeyword { get; set; }

    [JsonPropertyName("sortProperty")]
    public string? SortProperty { get; set; }

    [JsonPropertyName("sortOrder")]
    public string? SortOrder { get; set; }

    [JsonPropertyName("accountingYearId")]
    public int? AccountingYearId { get; set; }
}

