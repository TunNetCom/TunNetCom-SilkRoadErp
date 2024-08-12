namespace TunNetCom.SilkRoadErp.Sales.WebApp.Services.Product;

public interface IProductsApiClient
{
    Task<OneOf<ProductResponse, bool>> GetProduct(string refe, CancellationToken cancellationToken);
    Task<PagedList<ProductResponse>> GetProducts(QueryStringParameters queryParameters, CancellationToken cancellationToken);
    Task<OneOf<CreateProductRequest, BadRequestResponse>> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken);
    Task<OneOf<ResponseTypes, BadRequestResponse>> UpdateProduct(UpdateProductRequest request, string refe, CancellationToken cancellationToken);
    Task<Stream> DeleteProduct(string refe, CancellationToken cancellationToken);
    Task<PagedList<ProductResponse>> SearchProducts(QueryStringParameters queryParameters, CancellationToken cancellationToken);
}
