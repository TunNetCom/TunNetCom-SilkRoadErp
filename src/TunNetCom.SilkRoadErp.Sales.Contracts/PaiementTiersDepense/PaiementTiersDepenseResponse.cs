namespace TunNetCom.SilkRoadErp.Sales.Contracts.PaiementTiersDepense;

public class PaiementTiersDepenseResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numeroTransactionBancaire")]
    public string? NumeroTransactionBancaire { get; set; }

    [JsonPropertyName("tiersDepenseFonctionnementId")]
    public int TiersDepenseFonctionnementId { get; set; }

    [JsonPropertyName("tiersDepenseFonctionnementNom")]
    public string? TiersDepenseFonctionnementNom { get; set; }

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("accountingYear")]
    public int AccountingYear { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("datePaiement")]
    public DateTime DatePaiement { get; set; }

    [JsonPropertyName("methodePaiement")]
    public string MethodePaiement { get; set; } = string.Empty;

    [JsonPropertyName("factureDepenseIds")]
    public List<int> FactureDepenseIds { get; set; } = new();

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

    [JsonPropertyName("dateModification")]
    public DateTime? DateModification { get; set; }
}
