namespace TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

/// <summary>
/// DTO returned by the solde client calculation service for a single client.
/// </summary>
public class SoldeClientCalculDto
{
    public decimal TotalFactures { get; set; }
    public decimal TotalBonsLivraisonNonFactures { get; set; }
    public decimal TotalAvoirs { get; set; }
    public decimal TotalFacturesAvoir { get; set; }
    public decimal TotalPaiements { get; set; }
    public decimal Solde { get; set; }
}
