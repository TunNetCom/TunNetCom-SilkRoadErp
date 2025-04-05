namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSource;

public record RetenuSourcePrintModel
{
    public string? CustomerName { get; set; }
    public string? CustomerTel { get; set; }
    public string? CustomerAdresse { get; set; }
    public string? CustomerMatricule { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerCodeCat { get; set; }
    public string? CustomerEtbSec { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyAdress { get; set; }
    public string? CompanyMatricule { get; set; }
    public string? CompanyCodeCat { get; set; }
    public string? CompanyCodeTVA { get; set; }
    public string? CompanyEtbSec { get; set; }
    public required List<InvoiceResponse> Invoices { get; set; }
}
 