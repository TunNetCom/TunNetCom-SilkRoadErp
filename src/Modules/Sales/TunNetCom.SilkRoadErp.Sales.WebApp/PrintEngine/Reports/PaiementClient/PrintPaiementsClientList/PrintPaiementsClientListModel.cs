using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.PaiementClient.PrintPaiementsClientList;

public class PrintPaiementsClientListModel
{
    public PrintCompanyInfo Company { get; set; } = new();
    public List<PrintPaiementClientRow> Rows { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int DecimalPlaces { get; set; }
    public decimal TotalMontant { get; set; }
}

public class PrintPaiementClientRow
{
    public string ClientNom { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public DateTime DatePaiement { get; set; }
    public string MethodePaiement { get; set; } = string.Empty;
    public DateTime? DateEcheance { get; set; }
}
