using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFactureDepense;

public class CreateRetenueSourceFactureDepenseRequest
{
    [JsonPropertyName("factureDepenseId")]
    public int FactureDepenseId { get; set; }

    [JsonPropertyName("numTej")]
    public string? NumTej { get; set; }

    [JsonPropertyName("pdfContent")]
    public string? PdfContent { get; set; }
}
