namespace TunNetCom.SilkRoadErp.Sales.WebApp.PrintEngine.Infrastructure;

public interface IPrintService
{
    Task<PrintResult> PrintAsync(byte[] pdfBytes, PrintOptions options, CancellationToken cancellationToken = default);
}

public class PrintOptions
{
    public string? PrinterName { get; set; }
    public int Copies { get; set; } = 1;
    public PrintMode Mode { get; set; } = PrintMode.Download;
    public bool Duplex { get; set; } = false;
    public string? FileName { get; set; }
}

public enum PrintMode
{
    Download,
    DirectPrint
}

public class PrintResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? JobId { get; set; }

    public static PrintResult Success(string? jobId = null) => new() { IsSuccess = true, JobId = jobId };
    public static PrintResult Failure(string errorMessage) => new() { IsSuccess = false, ErrorMessage = errorMessage };
}

