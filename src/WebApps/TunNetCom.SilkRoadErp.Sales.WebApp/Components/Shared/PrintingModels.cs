using TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Components.Shared;

public class PrinterSelectionResult
{
    public string PrinterName { get; set; } = string.Empty;
    public int Copies { get; set; } = 1;
    public bool Duplex { get; set; } = false;
    public byte[]? PdfBytes { get; set; }
}

public class PrintModeResult
{
    public PrintMode Mode { get; set; }
    public bool IncludeHeader { get; set; } = true;
}

