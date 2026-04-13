using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.ValidateInvoices;

public record ValidateInvoicesCommand(List<int> Ids) : IRequest<Result>;
