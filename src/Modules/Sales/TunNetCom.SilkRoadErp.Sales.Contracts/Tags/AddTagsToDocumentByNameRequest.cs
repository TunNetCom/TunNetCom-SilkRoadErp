using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

public class AddTagsToDocumentByNameRequest
{
    [Required]
    [JsonPropertyName("tagNames")]
    public List<string> TagNames { get; set; } = new();
}

