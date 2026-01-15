using FluentResults;
using MediatR;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.TransferInvoiceToCustomer;

public record TransferInvoiceToCustomerCommand(
    int InvoiceNumber,
    int TargetCustomerId
) : IRequest<Result>;

