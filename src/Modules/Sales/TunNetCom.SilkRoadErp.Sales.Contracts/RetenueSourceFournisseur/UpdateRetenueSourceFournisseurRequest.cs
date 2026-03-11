using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

public class UpdateRetenueSourceFournisseurRequest
{
    [JsonPropertyName("numTej")]
    public string? NumTej { get; set; }

    [JsonPropertyName("pdfContent")]
    public string? PdfContent { get; set; } // Base64 encoded PDF content
}


