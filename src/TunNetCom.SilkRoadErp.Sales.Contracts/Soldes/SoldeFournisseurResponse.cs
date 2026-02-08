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

    [JsonPropertyName("totalAvoirsFinanciers")]
    public decimal TotalAvoirsFinanciers { get; set; }

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

    /// <summary>Formule du montant (pour les factures fournisseur : ((BR - timbre) - avoirs) × (1 - taux retenue) + timbre).</summary>
    [JsonPropertyName("formuleMontant")]
    public string? FormuleMontant { get; set; }

    /// <summary>Avoirs rattachés à cette facture (avoir financier, facture avoir) pour affichage dans la liste des documents.</summary>
    [JsonPropertyName("avoirsRattaches")]
    public List<AvoirRattacheSolde>? AvoirsRattaches { get; set; }
}

/// <summary>Résumé d'un avoir rattaché à une facture fournisseur (type + numéro/libellé + montant).</summary>
public class AvoirRattacheSolde
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty; // "AvoirFinancier" ou "FactureAvoir"

    [JsonPropertyName("numero")]
    public int Numero { get; set; }

    [JsonPropertyName("libelle")]
    public string? Libelle { get; set; }

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

public class SoldeTiersDepenseResponse
{
    [JsonPropertyName("tiersDepenseFonctionnementId")]
    public int TiersDepenseFonctionnementId { get; set; }

    [JsonPropertyName("tiersDepenseFonctionnementNom")]
    public string TiersDepenseFonctionnementNom { get; set; } = string.Empty;

    [JsonPropertyName("accountingYearId")]
    public int AccountingYearId { get; set; }

    [JsonPropertyName("totalFacturesDepense")]
    public decimal TotalFacturesDepense { get; set; }

    [JsonPropertyName("totalPaiements")]
    public decimal TotalPaiements { get; set; }

    [JsonPropertyName("solde")]
    public decimal Solde { get; set; }

    [JsonPropertyName("documents")]
    public List<DocumentSoldeTiersDepense> Documents { get; set; } = new();

    [JsonPropertyName("paiements")]
    public List<PaiementSoldeTiersDepense> Paiements { get; set; } = new();
}

public class DocumentSoldeTiersDepense
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("numero")]
    public int Numero { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("montant")]
    public decimal Montant { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class PaiementSoldeTiersDepense
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
    public List<FactureDepenseRattacheeSolde> Factures { get; set; } = new();
}

public class FactureDepenseRattacheeSolde
{
    [JsonPropertyName("numero")]
    public int Numero { get; set; }

    [JsonPropertyName("montantTtc")]
    public decimal MontantTtc { get; set; }
}

