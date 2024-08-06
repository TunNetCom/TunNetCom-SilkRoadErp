using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;
namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Providers;

public interface IProvidersApiClient
{
    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateProvider(
        UpdateProviderRequest request,
        int id,
        CancellationToken cancellationToken);

    Task<OneOf<ProviderResponse, bool>> GetProvider(
        int id,
        CancellationToken cancellationToken);

    Task<Stream> DeleteProvider(string id, CancellationToken cancellationToken);

    Task<PagedList<ProviderResponse>> GetProviders(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);

    Task<OneOf<CreateProviderRequest, BadRequestResponse>> CreateProvider(
        CreateProviderRequest request,
        CancellationToken cancellationToken);
}
