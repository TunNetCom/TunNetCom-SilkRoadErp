namespace TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

public class CreatePaiementClientRequest
{
    [JsonPropertyName("numeroTransactionBancaire")]
    public string? NumeroTransactionBancaire { get; set; }

    [JsonPropertyName("clientId")]
    public int ClientId { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("datePaiement")]
    public DateTime DatePaiement { get; set; }

    [JsonPropertyName("methodePaiement")]
    public string MethodePaiement { get; set; } = string.Empty;

    [JsonPropertyName("factureIds")]
    public List<int>? FactureIds { get; set; }

    [JsonPropertyName("bonDeLivraisonIds")]
    public List<int>? BonDeLivraisonIds { get; set; }

    [JsonPropertyName("numeroChequeTraite")]
    public string? NumeroChequeTraite { get; set; }

    [JsonPropertyName("banqueId")]
    public int? BanqueId { get; set; }

    [JsonPropertyName("dateEcheance")]
    public DateTime? DateEcheance { get; set; }

    [JsonPropertyName("commentaire")]
    public string? Commentaire { get; set; }

    [JsonPropertyName("documentBase64")]
    public string? DocumentBase64 { get; set; }
}

