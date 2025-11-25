using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products;

public class ProductStockResponse
{
    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("stockInitial")]
    public int StockInitial { get; set; }

    [JsonPropertyName("totalAchats")]
    public int TotalAchats { get; set; }

    [JsonPropertyName("totalVentes")]
    public int TotalVentes { get; set; }

    [JsonPropertyName("stockCalcule")]
    public int StockCalcule { get; set; }

    [JsonPropertyName("stockDisponible")]
    public int StockDisponible { get; set; }
}

