namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.AvoirFournisseur;

/// <summary>
/// Modèle pour l'impression de l'avoir fournisseur
/// </summary>
public class PrintAvoirFournisseurModel
{
    public int Id { get; set; }
    public int NumAvoirChezFournisseur { get; set; }
    public DateTime Date { get; set; }

    public int? ProviderId { get; set; }
    public PrintAvoirFournisseurProviderInfo? Provider { get; set; }

    public string Statut { get; set; } = string.Empty;
    public string StatutLibelle { get; set; } = string.Empty;

    public List<PrintAvoirFournisseurLineModel> Lines { get; set; } = new();

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

public class PrintAvoirFournisseurProviderInfo
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? Tel { get; set; }
    public string? Matricule { get; set; }
    public string? Code { get; set; }
}

public class PrintAvoirFournisseurLineModel
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
