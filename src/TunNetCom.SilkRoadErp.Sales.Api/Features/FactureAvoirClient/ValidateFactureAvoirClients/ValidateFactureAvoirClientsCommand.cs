using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirClient.ValidateFactureAvoirClients;

public record ValidateFactureAvoirClientsCommand(List<int> Ids) : IRequest<Result>;