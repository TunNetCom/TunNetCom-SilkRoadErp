using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.AvoirFournisseur.ValidateAvoirFournisseurs;

public record ValidateAvoirFournisseursCommand(List<int> Ids) : IRequest<Result>;
