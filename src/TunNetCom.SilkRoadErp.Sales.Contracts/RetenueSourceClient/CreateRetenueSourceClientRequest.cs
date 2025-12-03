using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceClient;

public class CreateRetenueSourceClientRequest
{
    [JsonPropertyName("numFacture")]
    public int NumFacture { get; set; }

    [JsonPropertyName("numTej")]
    public string? NumTej { get; set; }

    [JsonPropertyName("pdfContent")]
    public string? PdfContent { get; set; } // Base64 encoded PDF content
}


