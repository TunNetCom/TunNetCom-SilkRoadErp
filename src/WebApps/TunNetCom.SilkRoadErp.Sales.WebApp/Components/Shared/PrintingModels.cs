namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Shared;

public class PrinterSelectionResult
{
    public string PrinterName { get; set; } = string.Empty;
    public int Copies { get; set; } = 1;
    public bool Duplex { get; set; } = false;
    public byte[]? PdfBytes { get; set; }
}

