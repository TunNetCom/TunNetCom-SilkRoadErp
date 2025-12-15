using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Quotations;

public class ValidateQuotationsRequest
{
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; } = new();
}

