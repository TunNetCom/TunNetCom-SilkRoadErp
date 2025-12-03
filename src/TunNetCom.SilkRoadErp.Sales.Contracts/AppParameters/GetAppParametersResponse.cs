using TunNetCom.SilkRoadErp.Sales.Contracts.Common;

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
    public decimal DiscountPercentage { get; set; }

    [JsonPropertyName("vatAmount")]
    public decimal VatAmount { get; set; }

    [JsonPropertyName("vatRate0")]
    public decimal VatRate0 { get; set; }

    [JsonPropertyName("vatRate7")]
    public decimal VatRate7 { get; set; }

    [JsonPropertyName("vatRate13")]
    public decimal VatRate13 { get; set; }

    [JsonPropertyName("vatRate19")]
    public decimal VatRate19 { get; set; }

    [JsonPropertyName("bloquerVenteStockInsuffisant")]
    public bool BloquerVenteStockInsuffisant { get; set; }

    [JsonPropertyName("decimalPlaces")]
    public int DecimalPlaces { get; set; } = DecimalFormatConstants.DEFAULT_DECIMAL_PLACES;

    [JsonPropertyName("seuilRetenueSource")]
    public decimal SeuilRetenueSource { get; set; } = 1000;
}
