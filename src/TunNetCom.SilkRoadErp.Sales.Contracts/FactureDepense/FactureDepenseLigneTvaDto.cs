using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.FactureDepense;

public class FactureDepenseLigneTvaDto
{
    [JsonPropertyName("tauxTVA")]
    public decimal TauxTVA { get; set; }

    [JsonPropertyName("baseHT")]
    public decimal BaseHT { get; set; }

    [JsonPropertyName("montantTVA")]
    public decimal MontantTVA { get; set; }
}
