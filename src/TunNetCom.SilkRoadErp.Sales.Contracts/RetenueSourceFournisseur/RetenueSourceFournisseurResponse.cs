using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.RetenueSourceFournisseur;

public class RetenueSourceFournisseurResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numFactureFournisseur")]
    public int NumFactureFournisseur { get; set; }

    [JsonPropertyName("numTej")]
    public string? NumTej { get; set; }

    [JsonPropertyName("montantAvantRetenu")]
    public decimal MontantAvantRetenu { get; set; }

    [JsonPropertyName("tauxRetenu")]
    public double TauxRetenu { get; set; }

    [JsonPropertyName("montantApresRetenu")]
    public decimal MontantApresRetenu { get; set; }

    [JsonPropertyName("dateCreation")]
    public DateTime DateCreation { get; set; }

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("hasPdf")]
    public bool HasPdf { get; set; }
}


