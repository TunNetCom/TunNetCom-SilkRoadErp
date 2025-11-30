using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Commandes.ValidateOrders;

public record ValidateOrdersCommand(List<int> Ids) : IRequest<Result>;



