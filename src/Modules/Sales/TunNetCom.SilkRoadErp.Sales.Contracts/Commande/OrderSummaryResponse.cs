namespace TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
public class OrderSummaryResponse
{
    public int OrderNumber { get; set; }
    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalExcludingVat { get; set; }
    public decimal TotalVat { get; set; }
    public decimal NetToPay { get; set; }
    public int Statut { get; set; }
    public string StatutLibelle { get; set; } = string.Empty;
}

