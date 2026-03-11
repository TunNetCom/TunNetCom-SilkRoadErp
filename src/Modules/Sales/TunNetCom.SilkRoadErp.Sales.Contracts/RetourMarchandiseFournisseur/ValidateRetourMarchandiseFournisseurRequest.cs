using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetourMarchandiseFournisseur;

public class ValidateRetourMarchandiseFournisseurRequest
{
    [JsonPropertyName("ids")]
    public List<int> Ids { get; set; } = new();
}

