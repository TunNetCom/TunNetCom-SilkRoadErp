namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.UpdateOrder;

public record UpdateOrderCommand(
    int Num,
    int? FournisseurId,
    DateTime Date,
    decimal TotHTva,
    decimal TotTva,
    decimal TotTtc,
    IEnumerable<CreateOrder.LigneCommandeSubCommand> OrderLines
) : IRequest<Result>;

