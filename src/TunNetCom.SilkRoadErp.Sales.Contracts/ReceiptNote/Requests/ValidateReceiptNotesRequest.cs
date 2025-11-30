using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNote.Requests;

public class ValidateReceiptNotesRequest
{
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; } = new();
}