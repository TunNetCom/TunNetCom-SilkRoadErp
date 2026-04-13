namespace TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

public class ProviderInvoiceTotalsResponse
{
    public decimal TotalHT { get; set; }
    public decimal TotalBase7 { get; set; }
    public decimal TotalBase13 { get; set; }
    public decimal TotalBase19 { get; set; }
    public decimal TotalVat7 { get; set; }
    public decimal TotalVat13 { get; set; }
    public decimal TotalVat19 { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalTTC { get; set; }
}

