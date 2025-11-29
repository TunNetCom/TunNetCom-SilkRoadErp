using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.ProviderInvoice; 

public interface IProviderInvoiceApiClient
{
    Task<OneOf<GetProviderInvoicesWithSummary, BadRequestResponse>> GetProvidersInvoicesAsync(
        int idFournisseur,
        QueryStringParameters query,
        CancellationToken cancellationToken = default);

    Task<List<ProviderInvoiceResponse>> GetProviderInvoicesByIdsAsync(
        List<int> invoicesIds,
        CancellationToken cancellationToken = default);

    Task<Result<FullProviderInvoiceResponse>> GetFullProviderInvoiceByIdAsync(
    int id,
    CancellationToken cancellationToken);

    Task<OneOf<int, BadRequestResponse>> CreateProviderInvoice(
        CreateProviderInvoiceRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ValidateProviderInvoicesAsync(List<int> ids, CancellationToken cancellationToken);
}