using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.UpdateProviderInvoiceDate;

public record UpdateProviderInvoiceDateCommand(int Num, DateTime Date) : IRequest<Result>;
