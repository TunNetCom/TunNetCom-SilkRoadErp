using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class ValidateInvoicesRequest
{
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; } = new();
}


