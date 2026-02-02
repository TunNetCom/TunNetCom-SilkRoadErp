namespace TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

public class SoldeFournisseurResponse
{
    [JsonPropertyName("fournisseurId")]
    public int FournisseurId { get; set; }

    [JsonPropertyName("fournisseurNom")]
    public string FournisseurNom { get; set; } = string.Empty;

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("totalFactures")]
    public decimal TotalFactures { get; set; }

    [JsonPropertyName("totalBonsReceptionNonFactures")]
    public decimal TotalBonsReceptionNonFactures { get; set; }

    [JsonPropertyName("totalFacturesAvoir")]
    public decimal TotalFacturesAvoir { get; set; }

    [JsonPropertyName("totalPaiements")]
    public decimal TotalPaiements { get; set; }

    [JsonPropertyName("solde")]
    public decimal Solde { get; set; }

    [JsonPropertyName("documents")]
    public List<DocumentSoldeFournisseur> Documents { get; set; } = new();

    [JsonPropertyName("paiements")]
    public List<PaiementSoldeFournisseur> Paiements { get; set; } = new();
}

public class DocumentSoldeFournisseur
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty; // "FactureFournisseur", "BonDeReception", ou "FactureAvoir"

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numero")]
    public int Numero { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }
}

public class PaiementSoldeFournisseur
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

    [JsonPropertyName("factures")]
    public List<FactureRattacheeSolde> Factures { get; set; } = new();
}

