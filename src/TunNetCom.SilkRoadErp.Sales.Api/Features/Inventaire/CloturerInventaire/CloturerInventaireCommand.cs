using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.CloturerInventaire;

public record CloturerInventaireCommand(int Id) : IRequest<Result>;

