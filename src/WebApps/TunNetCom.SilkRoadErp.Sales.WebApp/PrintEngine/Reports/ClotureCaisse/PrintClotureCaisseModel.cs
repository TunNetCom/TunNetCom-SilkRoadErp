using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.ClotureCaisse;

public class PrintClotureCaisseModel
{
    public PrintCompanyInfo Company { get; set; } = new();
    public DateTime DateDuJour { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int DecimalPlaces { get; set; }

    public List<PrintClotureCaissePaiementRow> PaiementsRows { get; set; } = new();
    public decimal SommeEspece { get; set; }
    public decimal SommeCheque { get; set; }
    public decimal SommeTraite { get; set; }
    public decimal SommeVirement { get; set; }
    public decimal TotalPaiements { get; set; }

    public List<PrintClotureCaisseBlRow> BlRows { get; set; } = new();
    public decimal TotalBL { get; set; }

    public List<PrintClotureCaisseAvoirRow> AvoirsRows { get; set; } = new();
    public decimal TotalAvoirs { get; set; }
}

public class PrintClotureCaissePaiementRow
{
    public string ClientNom { get; set; } = string.Empty;
    public DateTime DatePaiement { get; set; }
    public string MethodePaiement { get; set; } = string.Empty;
    public decimal Montant { get; set; }
}

public class PrintClotureCaisseBlRow
{
    public int Number { get; set; }
    public DateTime Date { get; set; }
    public string? CustomerName { get; set; }
    public decimal NetAmount { get; set; }
}

public class PrintClotureCaisseAvoirRow
{
    public int Num { get; set; }
    public DateTime Date { get; set; }
    public string? ClientName { get; set; }
    public decimal TotalIncludingTaxAmount { get; set; }
}
