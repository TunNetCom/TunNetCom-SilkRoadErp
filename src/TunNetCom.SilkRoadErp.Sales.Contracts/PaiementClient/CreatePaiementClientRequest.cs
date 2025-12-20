namespace TunNetCom.SilkRoadErp.Sales.Contracts.PaiementClient;

public class CreatePaiementClientRequest
{
    [JsonPropertyName("numero")]
    public string Numero { get; set; } = string.Empty;

    [JsonPropertyName("clientId")]
    public int ClientId { get; set; }

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

    [JsonPropertyName("dateEcheance")]
    public DateTime? DateEcheance { get; set; }

    [JsonPropertyName("commentaire")]
    public string? Commentaire { get; set; }

    [JsonPropertyName("documentBase64")]
    public string? DocumentBase64 { get; set; }
}

