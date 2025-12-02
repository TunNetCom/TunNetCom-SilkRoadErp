namespace TunNetCom.SilkRoadErp.Sales.Api.Features.Products.GetProduct;
public record GetProductQuery(
     int PageNumber,
     int PageSize,
     string? SearchKeyword,
     string? SortProprety,
     string? SortOrder,
     int? FamilleProduitId,
     int? SousFamilleProduitId,
     bool? Visibility,
     bool? StockLowOnly,
     int? StockCalculeMin,
     int? StockCalculeMax) : IRequest<PagedList<ProductResponse>> ;