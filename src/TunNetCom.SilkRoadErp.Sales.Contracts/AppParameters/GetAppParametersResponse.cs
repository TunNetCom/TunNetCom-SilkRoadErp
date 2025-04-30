namespace TunNetCom.SilkRoadErp.Sales.Contracts.AppParameters;

public class GetAppParametersResponse
{
    [JsonPropertyName("id")]
    public string NomSociete { get; set; } = null!;

    [JsonPropertyName("timbre")]
    public decimal Timbre { get; set; }

    [JsonPropertyName("adresse")]
    public string Adresse { get; set; } = null!;

    [JsonPropertyName("tel")]
    public string Tel { get; set; } = null!;

    [JsonPropertyName("fax")]
    public string? Fax { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("matriculeFiscale")]
    public string? MatriculeFiscale { get; set; }

    [JsonPropertyName("codeTva")]
    public string CodeTva { get; set; } = null!;

    [JsonPropertyName("codeCategorie")]
    public string? CodeCategorie { get; set; }

    [JsonPropertyName("etbSecondaire")]
    public string? EtbSecondaire { get; set; }

    [JsonPropertyName("pourcentageFodec")]
    public decimal PourcentageFodec { get; set; }

    [JsonPropertyName("adresseRetenu")]
    public string? AdresseRetenu { get; set; }

    [JsonPropertyName("pourcentageRetenu")]
    public double PourcentageRetenu { get; set; }

    [JsonPropertyName("discountPercentage")]
    public double DiscountPercentage { get; set; }

    [JsonPropertyName("vatAmount")]
    public double VatAmount { get; set; }
}
