using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Commande;

public class ValidateOrdersRequest
{
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; } = new();
}


