namespace TunNetCom.SilkRoadErp.Sales.Contracts.Invoice;

public class InvoiceTotalsResponse
{
    public decimal TotalHT { get; set; }
    public decimal TotalVat7 { get; set; }
    public decimal TotalVat13 { get; set; }
    public decimal TotalVat19 { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalTTC { get; set; }
}

