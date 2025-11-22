namespace TunNetCom.SilkRoadErp.Sales.Contracts.Printing;

public class PrinterInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? Location { get; set; }
}

public class PrintJobRequest
{
    public byte[] PdfBytes { get; set; } = Array.Empty<byte>();
    public string PrinterName { get; set; } = string.Empty;
    public int Copies { get; set; } = 1;
    public bool Duplex { get; set; } = false;
    public string? PaperSize { get; set; }
    public string? Orientation { get; set; }
}

public class PrintJobResponse
{
    public string? JobId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

