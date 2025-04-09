using TunNetCom.SilkRoadErp.Sales.Contracts.ProviderInvoice;
using TunNetCom.SilkRoadErp.Sales.Contracts.Providers;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Providers;

public interface IProvidersApiClient
{
    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync(
        UpdateProviderRequest request,
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<ProviderResponse, bool>> GetAsync(
        int id,
        CancellationToken cancellationToken);

    Task<Stream> DeleteAsync(int id, CancellationToken cancellationToken);

    Task<PagedList<ProviderResponse>> GetPagedAsync(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);

    Task<OneOf<CreateProviderRequest, BadRequestResponse>> CreateAsync(
        CreateProviderRequest request,
        CancellationToken cancellationToken);
    Task<PagedList<ProviderInvoiceResponse>> GetProvidersInvoicesAsync(
        int? idFournisseur = null,
        int pageNumber = 1,
        int pageSize = 10,
        string? searchKeyword = null,
        CancellationToken cancellationToken = default);
}
