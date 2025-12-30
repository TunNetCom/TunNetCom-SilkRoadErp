namespace TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

public class PaiementClientResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numeroTransactionBancaire")]
    public string? NumeroTransactionBancaire { get; set; }

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

    [JsonPropertyName("factureIds")]
    public List<int> FactureIds { get; set; } = new();

    [JsonPropertyName("bonDeLivraisonIds")]
    public List<int> BonDeLivraisonIds { get; set; } = new();

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

    [JsonPropertyName("documentStoragePath")]
    public string? DocumentStoragePath { get; set; }

    [JsonPropertyName("dateModification")]
    public DateTime? DateModification { get; set; }
}

