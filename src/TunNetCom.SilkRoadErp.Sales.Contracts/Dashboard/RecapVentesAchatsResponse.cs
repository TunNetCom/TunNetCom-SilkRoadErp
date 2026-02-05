namespace TunNetCom.SilkRoadErp.Sales.Contracts.Dashboard;

/// <summary>
/// API response for GET /api/dashboard/recap-ventes-achats. Mirrors the recap view model.
/// </summary>
public class RecapVentesAchatsResponse
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public RecapTotalsSectionDto VentesFactures { get; set; } = new();
    public RecapTotalsSectionDto VentesAvoirs { get; set; } = new();
    public RecapTotalsSectionDto VentesNettes { get; set; } = new();
    public RecapTotalsSectionDto AchatsFacturesFournisseurs { get; set; } = new();
    public RecapTotalsSectionDto AchatsAvoirsFournisseur { get; set; } = new();
    public decimal AchatsFacturesDepensesTotal { get; set; }
    public RecapTotalsSectionDto AchatsNets { get; set; } = new();
    public decimal PaiementsClientsTotal { get; set; }
    public decimal PaiementsFournisseursTotal { get; set; }
    public decimal PaiementsTiersDepensesTotal { get; set; }
    public RecapTotalsSectionDto Resultat { get; set; } = new();
}

public class RecapTotalsSectionDto
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
