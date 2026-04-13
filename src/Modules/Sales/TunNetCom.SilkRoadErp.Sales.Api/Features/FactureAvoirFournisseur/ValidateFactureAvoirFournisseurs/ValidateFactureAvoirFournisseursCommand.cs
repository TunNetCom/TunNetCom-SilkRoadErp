using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.FactureAvoirFournisseur.ValidateFactureAvoirFournisseurs;

public record ValidateFactureAvoirFournisseursCommand(List<int> Ids) : IRequest<Result>;
