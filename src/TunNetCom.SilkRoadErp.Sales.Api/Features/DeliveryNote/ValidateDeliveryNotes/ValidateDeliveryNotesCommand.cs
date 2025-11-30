using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.ValidateDeliveryNotes;

public record ValidateDeliveryNotesCommand(List<int> Ids) : IRequest<Result>;



