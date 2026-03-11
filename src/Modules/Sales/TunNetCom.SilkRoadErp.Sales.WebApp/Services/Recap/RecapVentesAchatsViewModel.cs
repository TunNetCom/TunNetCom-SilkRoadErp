namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Recap;

/// <summary>
/// View model for the Recap Ventes/Achats page. All amounts are for the selected period.
/// </summary>
public class RecapVentesAchatsViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    /// <summary>Factures clients: HT, TTC, TVA (and by rate if available).</summary>
    public RecapTotalsSection VentesFactures { get; set; } = new();

    /// <summary>Avoirs clients: HT, TTC, TVA.</summary>
    public RecapTotalsSection VentesAvoirs { get; set; } = new();

    /// <summary>Net ventes = Factures - Avoirs (HT, TTC, TVA).</summary>
    public RecapTotalsSection VentesNettes { get; set; } = new();

    /// <summary>Factures fournisseurs: HT, TTC, TVA.</summary>
    public RecapTotalsSection AchatsFacturesFournisseurs { get; set; } = new();

    /// <summary>Avoirs fournisseur: HT, TTC, TVA.</summary>
    public RecapTotalsSection AchatsAvoirsFournisseur { get; set; } = new();

    /// <summary>Factures dépenses: single total (TTC).</summary>
    public decimal AchatsFacturesDepensesTotal { get; set; }

    /// <summary>Net achats = Factures fournisseurs - Avoirs fournisseur + Factures dépenses.</summary>
    public RecapTotalsSection AchatsNets { get; set; } = new();

    /// <summary>Total paiements clients in period.</summary>
    public decimal PaiementsClientsTotal { get; set; }

    /// <summary>Total paiements fournisseurs in period.</summary>
    public decimal PaiementsFournisseursTotal { get; set; }

    /// <summary>Total paiements tiers dépenses in period.</summary>
    public decimal PaiementsTiersDepensesTotal { get; set; }

    /// <summary>Résultat = Ventes nettes - Achats nets (HT, TTC, TVA).</summary>
    public RecapTotalsSection Resultat { get; set; } = new();

    public bool HasError { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// HT, TTC, TVA totals for a section (and optional breakdown by rate).
/// </summary>
public class RecapTotalsSection
{
    public decimal TotalHT { get; set; }
    public decimal TotalTTC { get; set; }
    public decimal TotalTVA { get; set; }
    public decimal TotalBase7 { get; set; }
    public decimal TotalBase13 { get; set; }
    public decimal TotalBase19 { get; set; }
    public decimal TotalVat7 { get; set; }
    public decimal TotalVat13 { get; set; }
    public decimal TotalVat19 { get; set; }
}
