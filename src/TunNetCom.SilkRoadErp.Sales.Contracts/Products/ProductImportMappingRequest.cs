using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products;

public class ProductImportMappingRequest
{
    [JsonPropertyName("referenceColumn")]
    public string ReferenceColumn { get; set; } = null!;

    [JsonPropertyName("nameColumn")]
    public string? NameColumn { get; set; }

    [JsonPropertyName("prixBrutColumn")]
    public string PrixBrutColumn { get; set; } = null!;

    [JsonPropertyName("societeColumn")]
    public string? SocieteColumn { get; set; }

    [JsonPropertyName("sheetIndex")]
    public int SheetIndex { get; set; }
}
