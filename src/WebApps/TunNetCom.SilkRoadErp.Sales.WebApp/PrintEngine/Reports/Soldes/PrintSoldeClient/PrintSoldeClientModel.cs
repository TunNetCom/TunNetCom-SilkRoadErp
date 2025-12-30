using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Soldes.PrintSoldeClient;

public class PrintSoldeClientModel
{
    public PrintCompanyInfo Company { get; set; } = new();
    public PrintSoldeClientInfo Client { get; set; } = new();
    public PrintSoldeSummary Summary { get; set; } = new();
    public List<PrintSoldeClientDocument> Documents { get; set; } = new();
    public List<PrintSoldeClientPayment> Payments { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int DecimalPlaces { get; set; }
    public bool ShowOnlyMissingProducts { get; set; }
}

public class PrintSoldeClientInfo
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
    public string Matricule { get; set; } = string.Empty;
}

public class PrintSoldeSummary
{
    public decimal TotalFactures { get; set; }
    public decimal TotalBonsLivraisonNonFactures { get; set; }
    public decimal TotalAvoirs { get; set; }
    public decimal TotalFacturesAvoir { get; set; }
    public decimal TotalPaiements { get; set; }
    public decimal Solde { get; set; }
}

public class PrintSoldeClientDocument
{
    public string Type { get; set; } = string.Empty;
    public int Numero { get; set; }
    public DateTime Date { get; set; }
    public decimal Montant { get; set; }
    public bool HasMissingQuantities { get; set; }
    public List<PrintSoldeClientDeliveryNote> DeliveryNotes { get; set; } = new();
}

public class PrintSoldeClientDeliveryNote
{
    public int Numero { get; set; }
    public DateTime Date { get; set; }
    public decimal Montant { get; set; }
    public List<PrintSoldeClientDeliveryLine> Lines { get; set; } = new();
}

public class PrintSoldeClientDeliveryLine
{
    public string RefProduit { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public int? QuantiteLivree { get; set; }
    public int QuantiteNonLivree { get; set; }
}

public class PrintSoldeClientPayment
{
    public string? NumeroTransactionBancaire { get; set; }
    public DateTime DatePaiement { get; set; }
    public decimal Montant { get; set; }
    public string Methode { get; set; } = string.Empty;
}

