namespace TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

/// <summary>
/// DTO for one fournisseur in the bulk solde calculation (e.g. fournisseurs avec problème de solde).
/// </summary>
public class SoldeFournisseurItemDto
{
    public int FournisseurId { get; set; }
    public string FournisseurNom { get; set; } = string.Empty;
    public decimal TotalFactures { get; set; }
    public decimal TotalBonsReceptionNonFactures { get; set; }
    public decimal TotalFacturesAvoir { get; set; }
    public decimal TotalAvoirsFinanciers { get; set; }
    public decimal TotalPaiements { get; set; }
    public decimal Solde { get; set; }
    public DateTime? DateDernierDocument { get; set; }
}
