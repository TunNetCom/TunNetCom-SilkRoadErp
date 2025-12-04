namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products;

public class CreateProductRequest
{
    [JsonPropertyName("refe")]
    public string Refe { get; set; } = null!;

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = null!;

    [JsonPropertyName("qteLimite")]
    public int QteLimite { get; set; }

    [JsonPropertyName("remise")]
    public double Remise { get; set; }

    [JsonPropertyName("remiseAchat")]
    public double RemiseAchat { get; set; }

    [JsonPropertyName("tva")]
    public double Tva { get; set; }

    [JsonPropertyName("prix")]
    public decimal Prix { get; set; }

    [JsonPropertyName("prixAchat")]
    public decimal PrixAchat { get; set; }

    [JsonPropertyName("visibilite")]
    public bool Visibilite { get; set; }

    [JsonPropertyName("sousFamilleProduitId")]
    public int? SousFamilleProduitId { get; set; }

    [JsonPropertyName("image1Base64")]
    public string? Image1Base64 { get; set; }

    [JsonPropertyName("image2Base64")]
    public string? Image2Base64 { get; set; }

    [JsonPropertyName("image3Base64")]
    public string? Image3Base64 { get; set; }
}
