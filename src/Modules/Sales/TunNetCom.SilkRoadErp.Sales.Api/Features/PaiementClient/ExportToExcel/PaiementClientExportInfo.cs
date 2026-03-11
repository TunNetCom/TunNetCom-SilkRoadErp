namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PaiementClient.ExportToExcel;

public class PaiementClientExportInfo
{
    public string ClientNom { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public DateTime DatePaiement { get; set; }
    public string MethodePaiement { get; set; } = string.Empty;
    public DateTime? DateEcheance { get; set; }
    public string StatutReglement { get; set; } = string.Empty;
}
