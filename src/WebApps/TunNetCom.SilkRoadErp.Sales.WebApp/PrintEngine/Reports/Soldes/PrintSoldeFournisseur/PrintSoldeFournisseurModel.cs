using TunNetCom.SilkRoadErp.Sales.Contracts.Soldes;
using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes.PrintSoldeFournisseur;

public class PrintSoldeFournisseurModel
{
    public PrintCompanyInfo Company { get; set; } = new();
    public PrintSoldeFournisseurInfo Provider { get; set; } = new();
    public PrintSoldeFournisseurSummary Summary { get; set; } = new();
    public List<PrintSoldeFournisseurDocument> Documents { get; set; } = new();
    public List<PrintSoldeFournisseurPayment> Payments { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int DecimalPlaces { get; set; }
}

public class PrintSoldeFournisseurInfo
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
    public string Matricule { get; set; } = string.Empty;
}

public class PrintSoldeFournisseurSummary
{
    public decimal TotalFactures { get; set; }
    public decimal TotalBonsReceptionNonFactures { get; set; }
    public decimal TotalFacturesAvoir { get; set; }
    public decimal TotalAvoirsFinanciers { get; set; }
    public decimal TotalPaiements { get; set; }
    public decimal Solde { get; set; }
}

public class PrintSoldeFournisseurDocument
{
    public string Type { get; set; } = string.Empty;
    public int Numero { get; set; }
    public DateTime Date { get; set; }
    public decimal Montant { get; set; }
}

public class PrintSoldeFournisseurPayment
{
    public string? NumeroTransactionBancaire { get; set; }
    public DateTime DatePaiement { get; set; }
    public decimal Montant { get; set; }
    public string Methode { get; set; } = string.Empty;
    public List<FactureRattacheeSolde> Factures { get; set; } = new();
}

