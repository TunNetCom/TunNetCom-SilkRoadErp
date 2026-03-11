namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.Avoirs;

/// <summary>
/// Modèle pour l'impression de l'avoir client
/// </summary>
public class PrintAvoirModel
{
    public int Num { get; set; }
    public DateTime Date { get; set; }

    public int? ClientId { get; set; }
    public PrintAvoirClientInfo? Client { get; set; }

    public List<PrintAvoirLineModel> Lines { get; set; } = new();

    public decimal TotalExcludingTax { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalTTC { get; set; }

    public decimal Base19 { get; set; }
    public decimal Base13 { get; set; }
    public decimal Base7 { get; set; }
    public decimal Tva19 { get; set; }
    public decimal Tva13 { get; set; }
    public decimal Tva7 { get; set; }

    public double VatRate0 { get; set; }
    public double VatRate7 { get; set; }
    public double VatRate13 { get; set; }
    public double VatRate19 { get; set; }
}

public class PrintAvoirClientInfo
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? Tel { get; set; }
    public string? Matricule { get; set; }
    public string? Code { get; set; }
}

public class PrintAvoirLineModel
{
    public int IdLi { get; set; }
    public string RefProduit { get; set; } = string.Empty;
    public string DesignationLi { get; set; } = string.Empty;
    public int QteLi { get; set; }
    public decimal PrixHt { get; set; }
    public double Remise { get; set; }
    public decimal TotHt { get; set; }
    public double Tva { get; set; }
    public decimal TotTtc { get; set; }
}
