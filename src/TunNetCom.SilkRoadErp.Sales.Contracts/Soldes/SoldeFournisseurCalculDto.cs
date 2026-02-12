namespace TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

/// <summary>
/// DTO returned by the solde fournisseur calculation service for a single fournisseur.
/// </summary>
public class SoldeFournisseurCalculDto
{
    public decimal TotalFactures { get; set; }
    public decimal TotalBonsReceptionNonFactures { get; set; }
    public decimal TotalFacturesAvoir { get; set; }
    public decimal TotalAvoirsFinanciers { get; set; }
    public decimal TotalPaiements { get; set; }
    public decimal Solde { get; set; }
}
