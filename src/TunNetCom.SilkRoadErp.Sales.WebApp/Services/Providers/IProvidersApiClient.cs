namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Providers;

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
}
