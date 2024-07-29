using System.Text.Json.Serialization;


namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products;

public class CreateProductRequest
{
    [JsonPropertyName("refe")]
    public string Refe { get; set; } = null!;

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = null!;

    [JsonPropertyName("qte")]
    public int Qte { get; set; }

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
}
