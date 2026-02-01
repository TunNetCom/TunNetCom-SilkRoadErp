namespace TunNetCom.SilkRoadErp.Sales.Contracts.PaiementFournisseur;

public class CreatePaiementFournisseurRequest
{
    [JsonPropertyName("numeroTransactionBancaire")]
    public string? NumeroTransactionBancaire { get; set; }

    [JsonPropertyName("fournisseurId")]
    public int FournisseurId { get; set; }

    [JsonPropertyName("accountingYearId")]
    public int? AccountingYearId { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("datePaiement")]
    public DateTime DatePaiement { get; set; }

    [JsonPropertyName("methodePaiement")]
    public string MethodePaiement { get; set; } = string.Empty;

    [JsonPropertyName("factureFournisseurIds")]
    public List<int>? FactureFournisseurIds { get; set; }

    [JsonPropertyName("bonDeReceptionIds")]
    public List<int>? BonDeReceptionIds { get; set; }

    [JsonPropertyName("numeroChequeTraite")]
    public string? NumeroChequeTraite { get; set; }

    [JsonPropertyName("banqueId")]
    public int? BanqueId { get; set; }

    [JsonPropertyName("dateEcheance")]
    public DateTime? DateEcheance { get; set; }

    [JsonPropertyName("commentaire")]
    public string? Commentaire { get; set; }

    [JsonPropertyName("ribCodeEtab")]
    public string? RibCodeEtab { get; set; }

    [JsonPropertyName("ribCodeAgence")]
    public string? RibCodeAgence { get; set; }

    [JsonPropertyName("ribNumeroCompte")]
    public string? RibNumeroCompte { get; set; }

    [JsonPropertyName("ribCle")]
    public string? RibCle { get; set; }

    [JsonPropertyName("mois")]
    public int? Mois { get; set; }

    [JsonPropertyName("documentBase64")]
    public string? DocumentBase64 { get; set; }
}

