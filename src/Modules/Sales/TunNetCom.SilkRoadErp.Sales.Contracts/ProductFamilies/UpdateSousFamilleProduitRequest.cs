using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

public class UpdateSousFamilleProduitRequest
{
    [JsonPropertyName("nom")]
    public string Nom { get; set; } = null!;

    [JsonPropertyName("familleProduitId")]
    public int FamilleProduitId { get; set; }
}

