using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Avoirs.ValidateAvoirs;

public record ValidateAvoirsCommand(List<int> Ids) : IRequest<Result>;



