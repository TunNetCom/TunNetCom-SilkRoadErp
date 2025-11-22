namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public interface IPrinterService
{
    Task<List<PrinterInfo>> GetAvailablePrintersAsync(CancellationToken cancellationToken = default);
    Task<PrintResult> PrintAsync(byte[] pdfBytes, string printerName, PrintJobOptions options, CancellationToken cancellationToken = default);
}

public class PrinterInfo
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? Location { get; set; }
}

public class PrintJobOptions
{
    public int Copies { get; set; } = 1;
    public bool Duplex { get; set; } = false;
    public string? PaperSize { get; set; }
    public string? Orientation { get; set; }
}

