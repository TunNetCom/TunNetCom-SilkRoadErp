using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Invoices.UpdateInvoiceDate;

public record UpdateInvoiceDateCommand(int Num, DateTime Date) : IRequest<Result>;
