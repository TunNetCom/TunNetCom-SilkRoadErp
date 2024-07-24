namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProduct;
public record GetProductQuery(
     int PageNumber,
     int PageSize,
     string? SearchKeyword) : IRequest<PagedList<ProductResponse>> ;