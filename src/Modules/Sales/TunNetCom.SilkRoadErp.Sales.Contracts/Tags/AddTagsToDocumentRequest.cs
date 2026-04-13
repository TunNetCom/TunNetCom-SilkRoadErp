using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

public class AddTagsToDocumentRequest
{
    [Required]
    [JsonPropertyName("tagIds")]
    public List<int> TagIds { get; set; } = new();
}

