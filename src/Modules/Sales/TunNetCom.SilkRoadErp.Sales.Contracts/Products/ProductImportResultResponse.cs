using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products;

public class ProductImportResultResponse
{
    [JsonPropertyName("createdCount")]
    public int CreatedCount { get; set; }

    [JsonPropertyName("updatedCount")]
    public int UpdatedCount { get; set; }

    [JsonPropertyName("errorCount")]
    public int ErrorCount { get; set; }

    [JsonPropertyName("errors")]
    public IReadOnlyList<string> Errors { get; set; } = new List<string>();
}
