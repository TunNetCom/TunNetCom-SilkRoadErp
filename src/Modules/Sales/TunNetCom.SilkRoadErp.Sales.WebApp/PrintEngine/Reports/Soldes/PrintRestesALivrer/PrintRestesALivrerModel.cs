using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes.PrintRestesALivrer;

public class PrintRestesALivrerModel
{
    public PrintCompanyInfo Company { get; set; } = new();
    public List<ClientRestesALivrerPrintItem> Clients { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int DecimalPlaces { get; set; }
}

public class ClientRestesALivrerPrintItem
{
    public string ClientNom { get; set; } = string.Empty;
    public decimal Solde { get; set; }
    public List<LigneResteaLivrerPrint> Lignes { get; set; } = new();
}

public class LigneResteaLivrerPrint
{
    public string RefProduit { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int QuantiteRestante { get; set; }
}
