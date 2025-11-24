using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.ValiderInventaire;

public record ValiderInventaireCommand(int Id) : IRequest<Result>;

