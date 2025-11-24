using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

public class UpdateInventaireRequest
{
    [JsonPropertyName("dateInventaire")]
    public DateTime DateInventaire { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("lignes")]
    public List<UpdateLigneInventaireRequest> Lignes { get; set; } = new();
}

public class UpdateLigneInventaireRequest
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("quantiteReelle")]
    public int QuantiteReelle { get; set; }

    [JsonPropertyName("prixHt")]
    public decimal PrixHt { get; set; }

    [JsonPropertyName("dernierPrixAchat")]
    public decimal DernierPrixAchat { get; set; }
}

