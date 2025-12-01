using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.ValidateProviderInvoices;

public record ValidateProviderInvoicesCommand(List<int> Ids) : IRequest<Result>;
