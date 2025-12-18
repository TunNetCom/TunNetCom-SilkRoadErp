using MediatR;
using FluentResults;

namespace TunNetCom.SilkRoadErp.Sales.Api.Features.ProviderInvoice.CreateProviderInvoice;

public record CreateProviderInvoiceCommand(
        DateTime Date,
        int ProviderId,
        long NumFactureFournisseur
    ) : IRequest<Result<int>>;

