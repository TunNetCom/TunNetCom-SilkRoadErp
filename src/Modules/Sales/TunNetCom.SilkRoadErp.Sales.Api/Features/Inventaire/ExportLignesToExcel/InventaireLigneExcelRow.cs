namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.ExportLignesToExcel;

public class InventaireLigneExcelRow
{
    public int NumInventaire { get; set; }

    public DateTime DateInventaire { get; set; }

    public string DescriptionInventaire { get; set; } = string.Empty;

    public string StatutLibelle { get; set; } = string.Empty;

    public int ExerciceComptable { get; set; }

    public string RefProduit { get; set; } = string.Empty;

    public string NomProduit { get; set; } = string.Empty;

    public int QuantiteReelle { get; set; }

    public decimal PrixHt { get; set; }

    public decimal MontantLigne { get; set; }
}
