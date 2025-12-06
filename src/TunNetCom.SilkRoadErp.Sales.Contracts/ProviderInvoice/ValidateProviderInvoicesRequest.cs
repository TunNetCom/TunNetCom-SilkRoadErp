using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class ValidateProviderInvoicesRequest
{
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; } = new();
}

