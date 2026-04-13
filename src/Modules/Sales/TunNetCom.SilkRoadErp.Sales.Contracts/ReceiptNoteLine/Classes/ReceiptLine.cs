namespace TunNetCom.SilkRoadErp.Sales.Contracts.ReceiptNoteLine.Classes;

public class ReceiptLine
{
    public int LineId { get; set; }
    public string ProductReference { get; set; } = null!;
    public string ItemDescription { get; set; } = null!;
    public int ItemQuantity { get; set; }
    public decimal UnitPriceExcludingTax { get; set; }
    public double Discount { get; set; }
    public decimal TotalExcludingTax { get; set; }
    public double VatRate { get; set; }
    public decimal TotalIncludingTax { get; set; }
}
