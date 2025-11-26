namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Orders.PrintOrder;

public class PrintOrderModel
{
    public int Num { get; set; }
    public DateTime Date { get; set; }
    public int? SupplierId { get; set; }
    public decimal TotalExcludingTax { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalAmount { get; set; }

    public OrderSupplierModel? Supplier { get; set; }
    public List<OrderLineModel> Lines { get; set; } = new();
}

public class OrderSupplierModel
{
    public int Id { get; set; }
    public string Nom { get; set; } = null!;
    public string? Tel { get; set; }
    public string? Adresse { get; set; }
    public string? Matricule { get; set; }
    public string? Code { get; set; }
}

public class OrderLineModel
{
    public int Id { get; set; }
    public string RefProduit { get; set; } = null!;
    public string DesignationLi { get; set; } = null!;
    public int QteLi { get; set; }
    public decimal PrixHt { get; set; }
    public double Remise { get; set; }
    public decimal TotHt { get; set; }
    public double Tva { get; set; }
    public decimal TotTtc { get; set; }
}

