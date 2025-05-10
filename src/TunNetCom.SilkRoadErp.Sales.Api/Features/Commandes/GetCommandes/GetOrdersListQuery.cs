using TunNetCom.SilkRoadErp.Sales.Contracts.Commande;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.GetCommandes;
public record GetOrdersListQuery : IRequest<Result<List<OrderSummaryResponse>>>;

