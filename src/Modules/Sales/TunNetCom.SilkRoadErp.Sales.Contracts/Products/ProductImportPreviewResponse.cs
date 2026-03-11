using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products;

public class ProductImportPreviewResponse
{
    [JsonPropertyName("sheetNames")]
    public IReadOnlyList<string> SheetNames { get; set; } = new List<string>();

    [JsonPropertyName("selectedSheetIndex")]
    public int SelectedSheetIndex { get; set; }

    [JsonPropertyName("headers")]
    public IReadOnlyList<string> Headers { get; set; } = new List<string>();

    [JsonPropertyName("rows")]
    public IReadOnlyList<Dictionary<string, object?>> Rows { get; set; } = new List<Dictionary<string, object?>>();
}
