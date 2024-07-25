using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Providers;

public class ProviderResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom")]
    public string Nom { get; set; } = null!;

    [JsonPropertyName("tel")]
    public string Tel { get; set; } = null!;

    [JsonPropertyName("fax")]
    public string? Fax { get; set; }

    [JsonPropertyName("matricule")]
    public string? Matricule { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("codecat")]
    public string? CodeCat { get; set; }

    [JsonPropertyName("etbsec")]
    public string? EtbSec { get; set; }

    [JsonPropertyName("mail")]
    public string? Mail { get; set; }

    [JsonPropertyName("maildeux")]
    public string? MailDeux { get; set; }

    [JsonPropertyName("constructeur")]
    public bool Constructeur { get; set; }

    [JsonPropertyName("adresse")]
    public string? Adresse { get; set; }
}
