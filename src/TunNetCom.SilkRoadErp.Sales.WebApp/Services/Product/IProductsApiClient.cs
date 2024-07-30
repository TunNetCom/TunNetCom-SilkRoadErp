using TunNetCom.SilkRoadErp.Sales.Contracts.Products;
using TunNetCom.SilkRoadErp.Sales.WebApp.Helpers;

namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Product;

public interface IProductsApiClient
{
    Task<OneOf<ProductResponse, bool>> GetProduct(int id, CancellationToken cancellationToken);
    Task<PagedList<ProductResponse>> GetProducts(QueryStringParameters queryParameters, CancellationToken cancellationToken);
    Task<OneOf<CreateProductRequest, BadRequestResponse>> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken);
    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateProduct(UpdateProductRequest request, int id, CancellationToken cancellationToken);
    Task<Stream> DeleteProduct(int id, CancellationToken cancellationToken);
}
