using System;
using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

public class InventaireResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("num")]
    public int Num { get; set; }

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("accountingYear")]
    public int AccountingYear { get; set; }

    [JsonPropertyName("dateInventaire")]
    public DateTime DateInventaire { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("statut")]
    public int Statut { get; set; }

    [JsonPropertyName("statutLibelle")]
    public string StatutLibelle { get; set; } = string.Empty;

    [JsonPropertyName("totalHt")]
    public decimal TotalHt { get; set; }
}

