using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

public class UpdateFamilleProduitRequest
{
    [JsonPropertyName("nom")]
    public string Nom { get; set; } = null!;
}

