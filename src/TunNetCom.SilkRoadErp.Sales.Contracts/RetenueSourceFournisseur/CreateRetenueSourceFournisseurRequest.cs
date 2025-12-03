using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

public class CreateRetenueSourceFournisseurRequest
{
    [JsonPropertyName("numFactureFournisseur")]
    public int NumFactureFournisseur { get; set; }

    [JsonPropertyName("numTej")]
    public string? NumTej { get; set; }

    [JsonPropertyName("pdfContent")]
    public string? PdfContent { get; set; } // Base64 encoded PDF content
}


