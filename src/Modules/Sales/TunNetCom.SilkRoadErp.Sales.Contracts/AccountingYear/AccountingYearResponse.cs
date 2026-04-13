using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.AccountingYear;

public class AccountingYearResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("timbre")]
    public decimal? Timbre { get; set; }

    [JsonPropertyName("pourcentageFodec")]
    public decimal? PourcentageFodec { get; set; }

    [JsonPropertyName("vatRate0")]
    public decimal? VatRate0 { get; set; }

    [JsonPropertyName("vatRate7")]
    public decimal? VatRate7 { get; set; }

    [JsonPropertyName("vatRate13")]
    public decimal? VatRate13 { get; set; }

    [JsonPropertyName("vatRate19")]
    public decimal? VatRate19 { get; set; }

    [JsonPropertyName("pourcentageRetenu")]
    public double? PourcentageRetenu { get; set; }

    [JsonPropertyName("vatAmount")]
    public decimal? VatAmount { get; set; }

    [JsonPropertyName("seuilRetenueSource")]
    public decimal? SeuilRetenueSource { get; set; }
}
