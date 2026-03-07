namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.FactureAvoirFournisseur;

/// <summary>
/// Modèle pour l'impression de la facture avoir fournisseur
/// </summary>
public class PrintFactureAvoirFournisseurModel
{
    public int Id { get; set; }
    public int NumFactureAvoirFourSurPage { get; set; }
    public DateTime Date { get; set; }

    public int IdFournisseur { get; set; }
    public PrintFactureAvoirFournisseurProviderInfo? Provider { get; set; }

    public string Statut { get; set; } = string.Empty;
    public string StatutLibelle { get; set; } = string.Empty;

    public List<PrintFactureAvoirFournisseurAvoirModel> AvoirFournisseurs { get; set; } = new();

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

public class PrintFactureAvoirFournisseurProviderInfo
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? Tel { get; set; }
    public string? Matricule { get; set; }
    public string? Code { get; set; }
}

public class PrintFactureAvoirFournisseurAvoirModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int NumAvoirChezFournisseur { get; set; }
    public List<PrintFactureAvoirFournisseurLineModel> Lines { get; set; } = new();
}

public class PrintFactureAvoirFournisseurLineModel
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
