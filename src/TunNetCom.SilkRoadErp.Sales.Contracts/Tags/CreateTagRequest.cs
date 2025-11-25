using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

public class CreateTagRequest
{
    [Required]
    [MaxLength(100)]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    [MaxLength(500)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

