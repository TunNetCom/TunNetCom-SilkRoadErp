using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Invoices.RetenueSourceFournisseur;

public class RetenuSourceFournisseurPrintModel
{
    public string? Nom { get; set; }
    public string? Tel { get; set; }
    public string? Fax { get; set; }
    public string? Matricule { get; set; }
    public string? Code { get; set; }
    public string? CodeCat { get; set; }
    public string? EtbSec { get; set; }
    public string? Mail { get; set; }
    public string? MailDeux { get; set; }
    public bool? Constructeur { get; set; }
    public string? Adresse { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyAdress { get; set; }
    public string? CompanyMatricule { get; set; }
    public string? CompanyCodeCat { get; set; }
    public string? CompanyCodeTVA { get; set; }
    public string? CompanyEtbSec { get; set; }
    public required List<ProviderInvoiceResponse> Invoices { get; set; }
}
