using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProductFamilies;

public class SousFamilleProduitResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = string.Empty;

    [JsonPropertyName("familleProduitId")]
    public int FamilleProduitId { get; set; }

    [JsonPropertyName("familleProduitNom")]
    public string? FamilleProduitNom { get; set; }
}

