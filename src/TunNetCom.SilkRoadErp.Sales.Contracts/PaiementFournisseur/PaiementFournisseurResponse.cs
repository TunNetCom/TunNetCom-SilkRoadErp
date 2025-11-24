namespace TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

public class PaiementFournisseurResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numero")]
    public string Numero { get; set; } = string.Empty;

    [JsonPropertyName("fournisseurId")]
    public int FournisseurId { get; set; }

    [JsonPropertyName("fournisseurNom")]
    public string? FournisseurNom { get; set; }

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("datePaiement")]
    public DateTime DatePaiement { get; set; }

    [JsonPropertyName("methodePaiement")]
    public string MethodePaiement { get; set; } = string.Empty;

    [JsonPropertyName("factureFournisseurId")]
    public int? FactureFournisseurId { get; set; }

    [JsonPropertyName("bonDeReceptionId")]
    public int? BonDeReceptionId { get; set; }

    [JsonPropertyName("numeroChequeTraite")]
    public string? NumeroChequeTraite { get; set; }

    [JsonPropertyName("banqueId")]
    public int? BanqueId { get; set; }

    [JsonPropertyName("banqueNom")]
    public string? BanqueNom { get; set; }

    [JsonPropertyName("dateEcheance")]
    public DateTime? DateEcheance { get; set; }

    [JsonPropertyName("commentaire")]
    public string? Commentaire { get; set; }

    [JsonPropertyName("dateModification")]
    public DateTime? DateModification { get; set; }
}

