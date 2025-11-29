namespace TunNetCom.SilkRoadErp.Sales.Contracts.Commande;
public class FullOrderResponse
{
    public int OrderNumber { get; set; }
    public DateTime Date { get; set; }
    public int? SupplierId { get; set; }
    public SupplierInfos Supplier { get; set; } = null!;
    public List<OrderLine> OrderLines { get; set; } = new();
    public decimal TotalExcludingVat { get; set; }
    public decimal TotalVat { get; set; }
    public decimal NetToPay { get; set; }
    public int Statut { get; set; }
    public string StatutLibelle { get; set; } = string.Empty;
}

public class SupplierInfos
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Code { get; set; }
    public string? CategoryCode { get; set; }
    public string? SecondaryEstablishment { get; set; }
    public string? Mail { get; set; }
}

public class OrderLine
{
    public int LineId { get; set; }
    public string? ProductReference { get; set; }
    public string? ItemDescription { get; set; }
    public decimal ItemQuantity { get; set; }
    public decimal UnitPriceExcludingTax { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalExcludingTax { get; set; }
    public decimal VatRate { get; set; }
    public decimal TotalIncludingTax { get; set; }
}

