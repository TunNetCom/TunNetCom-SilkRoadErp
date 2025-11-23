namespace TunNetCom.SilkRoadErp.Sales.Contracts.AvoirFournisseur;

public class FullAvoirFournisseurResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("fournisseurId")]
    public int? FournisseurId { get; set; }

    [JsonPropertyName("numFactureAvoirFournisseur")]
    public int? NumFactureAvoirFournisseur { get; set; }

    [JsonPropertyName("numAvoirFournisseur")]
    public int NumAvoirFournisseur { get; set; }

    [JsonPropertyName("fournisseur")]
    public AvoirFournisseurProviderResponse? Fournisseur { get; set; }

    [JsonPropertyName("lines")]
    public List<AvoirFournisseurLineResponse> Lines { get; set; } = new();

    [JsonPropertyName("totalExcludingTaxAmount")]
    public decimal TotalExcludingTaxAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }

    [JsonPropertyName("totalIncludingTaxAmount")]
    public decimal TotalIncludingTaxAmount { get; set; }
}

public class AvoirFournisseurProviderResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("fax")]
    public string? Fax { get; set; }

    [JsonPropertyName("registrationNumber")]
    public string? RegistrationNumber { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("categoryCode")]
    public string? CategoryCode { get; set; }

    [JsonPropertyName("secondaryEstablishment")]
    public string? SecondaryEstablishment { get; set; }

    [JsonPropertyName("mail")]
    public string? Mail { get; set; }

    [JsonPropertyName("adress")]
    public string? Adress { get; set; }
}

public class AvoirFournisseurLineResponse
{
    [JsonPropertyName("idLi")]
    public int IdLi { get; set; }

    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = null!;

    [JsonPropertyName("designationLi")]
    public string DesignationLi { get; set; } = null!;

    [JsonPropertyName("qteLi")]
    public int QteLi { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("remise")]
    public double Remise { get; set; }

    [JsonPropertyName("totHt")]
    public decimal TotHt { get; set; }

    [JsonPropertyName("tva")]
    public double Tva { get; set; }

    [JsonPropertyName("totTtc")]
    public decimal TotTtc { get; set; }
}

