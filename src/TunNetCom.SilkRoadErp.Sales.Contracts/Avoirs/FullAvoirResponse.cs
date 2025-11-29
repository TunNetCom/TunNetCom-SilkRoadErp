namespace TunNetCom.SilkRoadErp.Sales.Contracts.Avoirs;

public class FullAvoirResponse
{
    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("clientId")]
    public int? ClientId { get; set; }

    [JsonPropertyName("client")]
    public AvoirClientResponse? Client { get; set; }

    [JsonPropertyName("lines")]
    public List<AvoirLineResponse> Lines { get; set; } = new();

    [JsonPropertyName("totalExcludingTaxAmount")]
    public decimal TotalExcludingTaxAmount { get; set; }

    [JsonPropertyName("totalVATAmount")]
    public decimal TotalVATAmount { get; set; }

    [JsonPropertyName("totalIncludingTaxAmount")]
    public decimal TotalIncludingTaxAmount { get; set; }

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;
}

public class AvoirClientResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = null!;

    [JsonPropertyName("tel")]
    public string? Tel { get; set; }

    [JsonPropertyName("adresse")]
    public string? Adresse { get; set; }

    [JsonPropertyName("matricule")]
    public string? Matricule { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("codeCat")]
    public string? CodeCat { get; set; }

    [JsonPropertyName("etbSec")]
    public string? EtbSec { get; set; }

    [JsonPropertyName("mail")]
    public string? Mail { get; set; }
}

public class AvoirLineResponse
{
    [JsonPropertyName("idLi")]
    public int IdLi { get; set; }

    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = null!;

    [JsonPropertyName("designationLi")]
    public string? DesignationLi { get; set; }

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

