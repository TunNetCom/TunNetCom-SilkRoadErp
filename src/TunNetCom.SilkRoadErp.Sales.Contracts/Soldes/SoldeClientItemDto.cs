namespace TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;

/// <summary>
/// DTO for one client in the bulk solde calculation (e.g. clients avec problème de solde).
/// </summary>
public class SoldeClientItemDto
{
    public int ClientId { get; set; }
    public string ClientNom { get; set; } = string.Empty;
    public decimal TotalFactures { get; set; }
    public decimal TotalBonsLivraisonNonFactures { get; set; }
    public decimal TotalAvoirs { get; set; }
    public decimal TotalFacturesAvoir { get; set; }
    public decimal TotalPaiements { get; set; }
    public decimal Solde { get; set; }
    public DateTime? DateDernierDocument { get; set; }
}
