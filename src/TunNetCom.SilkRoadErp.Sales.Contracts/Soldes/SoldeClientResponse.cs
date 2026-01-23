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

    [JsonPropertyName("totalAvoirs")]
    public decimal TotalAvoirs { get; set; }

    [JsonPropertyName("totalFacturesAvoir")]
    public decimal TotalFacturesAvoir { get; set; }

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
    public string Type { get; set; } = string.Empty; // "Facture", "BonDeLivraison", "Avoir", ou "FactureAvoir"

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numero")]
    public int Numero { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("lignesBl")]
    public List<LigneBlSoldeClient>? LignesBl { get; set; }

    [JsonPropertyName("hasQuantitesNonLivrees")]
    public bool HasQuantitesNonLivrees { get; set; }

    [JsonPropertyName("bonsLivraison")]
    public List<DocumentSoldeClient>? BonsLivraison { get; set; }
}

public class LigneBlSoldeClient
{
    [JsonPropertyName("refProduit")]
    public string RefProduit { get; set; } = string.Empty;

    [JsonPropertyName("designationLi")]
    public string DesignationLi { get; set; } = string.Empty;

    [JsonPropertyName("qteLi")]
    public int QteLi { get; set; }

    [JsonPropertyName("qteLivree")]
    public int? QteLivree { get; set; }

    [JsonPropertyName("quantiteNonLivree")]
    public int QuantiteNonLivree { get; set; }
}

public class PaiementSoldeClient
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numeroTransactionBancaire")]
    public string? NumeroTransactionBancaire { get; set; }

    [JsonPropertyName("datePaiement")]
    public DateTime DatePaiement { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("methodePaiement")]
    public string MethodePaiement { get; set; } = string.Empty;

    [JsonPropertyName("numeroChequeTraite")]
    public string? NumeroChequeTraite { get; set; }

    [JsonPropertyName("banqueNom")]
    public string? BanqueNom { get; set; }

    [JsonPropertyName("dateEcheance")]
    public DateTime? DateEcheance { get; set; }
}

