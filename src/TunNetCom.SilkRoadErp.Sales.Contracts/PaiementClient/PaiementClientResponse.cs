namespace TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

public class PaiementClientResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numero")]
    public string Numero { get; set; } = string.Empty;

    [JsonPropertyName("clientId")]
    public int ClientId { get; set; }

    [JsonPropertyName("clientNom")]
    public string? ClientNom { get; set; }

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("datePaiement")]
    public DateTime DatePaiement { get; set; }

    [JsonPropertyName("methodePaiement")]
    public string MethodePaiement { get; set; } = string.Empty;

    [JsonPropertyName("factureId")]
    public int? FactureId { get; set; }

    [JsonPropertyName("bonDeLivraisonId")]
    public int? BonDeLivraisonId { get; set; }

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

