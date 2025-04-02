using TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Responses;

public class DeliveryNoteResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("totHT")]
    public decimal TotHT { get; set; }

    [JsonPropertyName("totTva")]
    public decimal TotTva { get; set; }

    [JsonPropertyName("netPayer")]
    public decimal NetPayer { get; set; }

    [JsonPropertyName("tempBl")]
    public TimeOnly TempBl { get; set; }

    [JsonPropertyName("numFacture")]
    public int? NumFacture { get; set; }

    [JsonPropertyName("clientId")]
    public int? ClientId { get; set; }

    [JsonPropertyName("lignes")]
    public List<LigneBlRequest> Lignes { get; set; }
}
