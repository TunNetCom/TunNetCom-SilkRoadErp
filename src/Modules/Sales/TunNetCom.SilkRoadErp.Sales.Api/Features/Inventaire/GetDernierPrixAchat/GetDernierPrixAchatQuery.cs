using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetDernierPrixAchat;

public record GetDernierPrixAchatQuery(string RefProduit) : IRequest<Result<decimal>>;

