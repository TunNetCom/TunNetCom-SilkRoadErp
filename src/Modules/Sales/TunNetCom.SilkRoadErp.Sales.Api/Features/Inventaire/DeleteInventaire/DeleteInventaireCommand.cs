using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.DeleteInventaire;

public record DeleteInventaireCommand(int Id) : IRequest<Result>;

