using FluentResults;
using MediatR;
using TunNetCom.SilkRoadErp.Sales.Contracts.Inventaire;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Inventaire.GetInventaireById;

public record GetInventaireByIdQuery(int Id) : IRequest<Result<FullInventaireResponse>>;

