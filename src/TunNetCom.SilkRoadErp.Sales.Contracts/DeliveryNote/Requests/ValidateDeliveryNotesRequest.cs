using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.DeliveryNote.Requests;

public class ValidateDeliveryNotesRequest
{
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; } = new();
}



