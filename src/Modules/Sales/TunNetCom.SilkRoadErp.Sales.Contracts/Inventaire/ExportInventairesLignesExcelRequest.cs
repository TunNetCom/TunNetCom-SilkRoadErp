using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

public class ExportInventairesLignesExcelRequest
{
    [JsonPropertyName("inventaireIds")]
    public List<int> InventaireIds { get; set; } = new();
}
