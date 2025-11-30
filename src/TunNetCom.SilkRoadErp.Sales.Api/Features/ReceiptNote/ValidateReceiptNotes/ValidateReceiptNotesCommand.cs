using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ReceiptNote.ValidateReceiptNotes;

public record ValidateReceiptNotesCommand(List<int> Ids) : IRequest<Result>;