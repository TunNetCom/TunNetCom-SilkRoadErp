using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

public class TagResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

