using FluentResults;
using TunNetCom.SilkRoadErp.Sales.Domain.Entites;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.PrintHistory.CreatePrintHistory;

public record CreatePrintHistoryCommand(
    string DocumentType,
    int DocumentId,
    PrintModeEnum PrintMode,
    int? UserId,
    string? Username,
    string? PrinterName = null,
    int Copies = 1,
    string? FileName = null,
    bool IsDuplicata = false) : IRequest<Result<long>>;