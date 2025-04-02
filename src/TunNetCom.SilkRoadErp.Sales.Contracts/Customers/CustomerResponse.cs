﻿namespace TunNetCom.SilkRoadErp.Sales.Contracts.Customers;

public class CustomerResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nom")]
    public string? Nom { get; set; }

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
