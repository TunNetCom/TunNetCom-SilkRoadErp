using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

public class CreateInventaireRequest
{
    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("dateInventaire")]
    public DateTime DateInventaire { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("lignes")]
    public List<CreateLigneInventaireRequest> Lignes { get; set; } = new();
}

public class CreateLigneInventaireRequest
{
    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("quantiteReelle")]
    public int QuantiteReelle { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("dernierPrixAchat")]
    public decimal DernierPrixAchat { get; set; }
}

