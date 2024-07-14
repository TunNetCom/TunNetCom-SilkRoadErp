using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

public class CreateCustomerRequest
{
    [JsonPropertyName("nom")]
    public string Nom { get; set; }

    [JsonPropertyName("tel")]
    public string Tel { get; set; }

    [JsonPropertyName("adresse")]
    public string Adresse { get; set; }

    [JsonPropertyName("matricule")]
    public string Matricule { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("codeCat")]
    public string CodeCat { get; set; }

    [JsonPropertyName("etbSec")]
    public string EtbSec { get; set; }

    [JsonPropertyName("mail")]
    public string Mail { get; set; }
}
