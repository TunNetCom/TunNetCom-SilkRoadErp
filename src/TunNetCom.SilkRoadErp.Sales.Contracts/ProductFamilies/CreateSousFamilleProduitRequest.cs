using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

public class CreateSousFamilleProduitRequest
{
    [JsonPropertyName("nom")]
    public string Nom { get; set; } = null!;

    [JsonPropertyName("familleProduitId")]
    public int FamilleProduitId { get; set; }
}

