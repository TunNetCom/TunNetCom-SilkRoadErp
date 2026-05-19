using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.DeliveryNote.DeliverRemaining;

public record DeliverRemainingQuantitiesCommand(
    int InvoiceId,
    int? DeliveryNoteNum,
    List<DeliverRemainingItemSubCommand> Items) : IRequest<Result<int>>;

public record DeliverRemainingItemSubCommand
{
    public string RefProduit { get; init; } = string.Empty;
    public string DesignationLi { get; init; } = string.Empty;
    public int QuantityToDeliver { get; init; }
}
