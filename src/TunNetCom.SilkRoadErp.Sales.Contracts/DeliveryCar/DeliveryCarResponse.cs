using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryCar;

public class DeliveryCarResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("matricule")]
    public string Matricule { get; set; } = string.Empty;

    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;
}

