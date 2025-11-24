namespace TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

public class SoldeClientResponse
{
    [JsonPropertyName("clientId")]
    public int ClientId { get; set; }

    [JsonPropertyName("clientNom")]
    public string ClientNom { get; set; } = string.Empty;

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("totalFactures")]
    public decimal TotalFactures { get; set; }

    [JsonPropertyName("totalBonsLivraisonNonFactures")]
    public decimal TotalBonsLivraisonNonFactures { get; set; }

    [JsonPropertyName("totalPaiements")]
    public decimal TotalPaiements { get; set; }

    [JsonPropertyName("solde")]
    public decimal Solde { get; set; }

    [JsonPropertyName("documents")]
    public List<DocumentSoldeClient> Documents { get; set; } = new();

    [JsonPropertyName("paiements")]
    public List<PaiementSoldeClient> Paiements { get; set; } = new();
}

public class DocumentSoldeClient
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty; // "Facture" ou "BonDeLivraison"

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numero")]
    public int Numero { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }
}

public class PaiementSoldeClient
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numero")]
    public string Numero { get; set; } = string.Empty;

    [JsonPropertyName("datePaiement")]
    public DateTime DatePaiement { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("methodePaiement")]
    public string MethodePaiement { get; set; } = string.Empty;
}

