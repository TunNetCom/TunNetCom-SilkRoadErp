using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

public class FactureDepenseTotalsResponse
{
    [JsonPropertyName("totalHT")]
    public decimal TotalHT { get; set; }

    [JsonPropertyName("totalTVA")]
    public decimal TotalTVA { get; set; }

    [JsonPropertyName("totalTTC")]
    public decimal TotalTTC { get; set; }
}
