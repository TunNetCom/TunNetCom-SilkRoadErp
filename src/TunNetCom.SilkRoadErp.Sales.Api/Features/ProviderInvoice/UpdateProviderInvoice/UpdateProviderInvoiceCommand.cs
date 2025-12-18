using MediatR;
using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.UpdateProviderInvoice;

public record UpdateProviderInvoiceCommand(
    int Num,
    long NumFactureFournisseur
) : IRequest<Result>;

