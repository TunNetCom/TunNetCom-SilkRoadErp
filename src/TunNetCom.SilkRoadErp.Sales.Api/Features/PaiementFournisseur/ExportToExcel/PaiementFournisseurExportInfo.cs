namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementFournisseur.ExportToExcel;

// Helper class for export data
public class PaiementFournisseurExportInfo
{
    public string NumeroTransactionBancaire { get; set; } = string.Empty;
    public string FournisseurNom { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public DateTime DatePaiement { get; set; }
    public string MethodePaiement { get; set; } = string.Empty;
    public string BanqueNom { get; set; } = string.Empty;
    public DateTime? DateEcheance { get; set; }
    public string StatutReglement { get; set; } = string.Empty;
}

