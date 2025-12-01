#nullable enable
using System;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public enum PrintModeEnum
{
    Download = 1,
    DirectPrint = 2
}

public class PrintHistory
{
    private PrintHistory()
    {
    }

    public static PrintHistory Create(
        string documentType,
        int documentId,
        PrintModeEnum printMode,
        int? userId,
        string? username,
        string? printerName = null,
        int copies = 1,
        string? fileName = null,
        bool isDuplicata = false)
    {
        return new PrintHistory
        {
            DocumentType = documentType,
            DocumentId = documentId,
            PrintMode = printMode,
            UserId = userId,
            Username = username ?? "System",
            PrinterName = printerName,
            Copies = copies,
            FileName = fileName,
            IsDuplicata = isDuplicata,
            PrintedAt = DateTime.UtcNow
        };
    }

    public void SetId(long id)
    {
        Id = id;
    }

    public long Id { get; private set; }

    public string DocumentType { get; private set; } = null!;

    public int DocumentId { get; private set; }

    public PrintModeEnum PrintMode { get; private set; }

    public int? UserId { get; private set; }

    public string Username { get; private set; } = null!;

    public DateTime PrintedAt { get; private set; }

    public string? PrinterName { get; private set; }

    public int Copies { get; private set; } = 1;

    public string? FileName { get; private set; }

    public bool IsDuplicata { get; private set; }
}
