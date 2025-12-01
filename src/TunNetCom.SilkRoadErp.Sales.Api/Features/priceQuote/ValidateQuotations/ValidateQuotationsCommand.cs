using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.priceQuote.ValidateQuotations;

public record ValidateQuotationsCommand(List<int> Ids) : IRequest<Result>;
