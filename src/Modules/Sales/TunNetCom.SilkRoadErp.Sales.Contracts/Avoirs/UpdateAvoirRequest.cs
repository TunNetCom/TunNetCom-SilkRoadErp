namespace TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

public class UpdateAvoirRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("clientId")]
    public int? ClientId { get; set; }

    [JsonPropertyName("lines")]
    public List<AvoirLineRequest> Lines { get; set; } = new();
}

