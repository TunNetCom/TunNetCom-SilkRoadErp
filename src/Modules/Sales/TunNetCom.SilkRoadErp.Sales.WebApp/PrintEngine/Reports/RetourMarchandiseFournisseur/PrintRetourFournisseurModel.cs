namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Reports.RetourMarchandiseFournisseur;

/// <summary>
/// Modèle pour l'impression du bon de retour fournisseur
/// </summary>
public class PrintRetourFournisseurModel
{
    public int Num { get; set; }
    public DateTime Date { get; set; }
    
    // Informations fournisseur
    public int? ProviderId { get; set; }
    public PrintProviderInfo? Provider { get; set; }
    
    // Statut
    public string Statut { get; set; } = string.Empty;
    public string StatutLibelle { get; set; } = string.Empty;
    
    // Lignes
    public List<PrintRetourLineModel> Lines { get; set; } = new();
    
    // Totaux
    public decimal TotalExcludingTax { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalTTC { get; set; }
    
    // Bases TVA par taux
    public decimal Base19 { get; set; }
    public decimal Base13 { get; set; }
    public decimal Base7 { get; set; }
    public decimal Tva19 { get; set; }
    public decimal Tva13 { get; set; }
    public decimal Tva7 { get; set; }
    
    // Taux TVA configurés
    public double VatRate0 { get; set; }
    public double VatRate7 { get; set; }
    public double VatRate13 { get; set; }
    public double VatRate19 { get; set; }
}

/// <summary>
/// Informations du fournisseur pour l'impression
/// </summary>
public class PrintProviderInfo
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? Tel { get; set; }
    public string? Matricule { get; set; }
    public string? Code { get; set; }
}

/// <summary>
/// Ligne de retour pour l'impression
/// </summary>
public class PrintRetourLineModel
{
    public int Id { get; set; }
    public string RefProduit { get; set; } = string.Empty;
    public string DesignationLi { get; set; } = string.Empty;
    public int QteLi { get; set; }
    public decimal PrixHt { get; set; }
    public double Remise { get; set; }
    public decimal TotHt { get; set; }
    public double Tva { get; set; }
    public decimal TotTtc { get; set; }
    
    // Informations de réception (optionnel)
    public int QteRecue { get; set; }
    public DateTime? DateReception { get; set; }
}
