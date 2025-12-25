namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

public class CreateAvoirFournisseurRequest
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("fournisseurId")]
    public int? FournisseurId { get; set; }

    [JsonPropertyName("numFactureAvoirFournisseur")]
    public int? NumFactureAvoirFournisseur { get; set; }

    [JsonPropertyName("numAvoirChezFournisseur")]
    public int NumAvoirChezFournisseur { get; set; }

    [JsonPropertyName("lines")]
    public List<AvoirFournisseurLineRequest> Lines { get; set; } = new();
}

public class AvoirFournisseurLineRequest
{
    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("designationLi")]
    public string DesignationLi { get; set; } = string.Empty;

    [JsonPropertyName("qteLi")]
    public int QteLi { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("remise")]
    public double Remise { get; set; }

    [JsonPropertyName("tva")]
    public double Tva { get; set; }
}

