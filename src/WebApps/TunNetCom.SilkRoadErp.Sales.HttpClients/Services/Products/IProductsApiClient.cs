using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
using TunNetCom.SilkRoadErp.Sales.HttpClients.Services;

namespace TunNetCom.SilkRoadErp.Sales.HttpClients.Services.Products;

public interface IProductsApiClient
{
    Task<OneOf<ProductResponse, bool>> GetAsync(
        string refe,
        CancellationToken cancellationToken);

    Task<PagedList<ProductResponse>> GetPagedAsync(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);

    Task<OneOf<CreateProductRequest, BadRequestResponse>> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateAsync
        (UpdateProductRequest request,
        string refe,
        CancellationToken cancellationToken);

    Task<OneOf<ResponseTypes, Stream>> DeleteAsync(
        string refe,
        CancellationToken cancellationToken);

    Task<PagedList<ProductResponse>> SearchProducts(
        QueryStringParameters queryParameters,
        CancellationToken cancellationToken);
}
