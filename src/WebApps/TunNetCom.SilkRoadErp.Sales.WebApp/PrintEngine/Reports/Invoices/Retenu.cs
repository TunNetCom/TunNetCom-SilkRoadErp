using System.Text.Json.Serialization;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices;

public record Retenu
{
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerTel { get; set; }
    public string? CustomerAdresse { get; set; }
    public string? CustomerMatricule { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerCodeCat { get; set; }
    public string? CustomerEtbSec { get; set; }
    public List<InvoiceResponse> Invoices { get; set; } 

}
 