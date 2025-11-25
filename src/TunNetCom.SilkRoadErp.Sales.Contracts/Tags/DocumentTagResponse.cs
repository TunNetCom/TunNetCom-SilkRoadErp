using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Tags;

public class DocumentTagResponse
{
    [JsonPropertyName("documentType")]
    public string DocumentType { get; set; } = string.Empty;

    [JsonPropertyName("documentId")]
    public int DocumentId { get; set; }

    [JsonPropertyName("tags")]
    public List<TagResponse> Tags { get; set; } = new();
}

